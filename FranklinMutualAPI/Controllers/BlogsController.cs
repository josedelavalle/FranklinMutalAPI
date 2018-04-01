using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FranklinMutualAPI.Models;
using HtmlAgilityPack;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace FranklinMutualAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Blogs")]
    public class BlogsController : Controller
    {
        private readonly FranklinMutualDbContext _context;

        HtmlWeb web = new HtmlWeb();

        public BlogsController(FranklinMutualDbContext context)
        {
            _context = context;
        }


        [HttpGet("[action]")]
        public IEnumerable<Blog> ImportData()
        {
            // get first page with all counties listed and url's to drill down to
            try
            {

            
                string url = "https://www.fmiweb.com/site/resource-center";
                HtmlAgilityPack.HtmlDocument doc = web.Load(url);
                
                // populate list of all counties
                IEnumerable<HtmlNode> blogList = doc.DocumentNode.SelectNodes("//div[@class='blog-result']");
                var y = 0;
                // loop through all counties found
                foreach (HtmlNode blog in blogList)
                {
                    // get node with needed data and navigate markup to get the data
                    List<HtmlNode> BlogNodes = blog.ChildNodes.Where(x => x.Name == "div").ToList();
                    string BlogImage = BlogNodes[0].ChildNodes.FirstOrDefault(n => n.Name == "div")
                        .ChildNodes.FirstOrDefault(x => x.Name == "a")
                        .ChildNodes.FirstOrDefault(x => x.Name == "img")
                        .Attributes[0].Value;
                    HtmlNode BlogTitle = BlogNodes[1].ChildNodes.FirstOrDefault(x => x.Name == "h2");
                    string BlogName = BlogTitle.InnerText;
                    string BlogUrl = BlogTitle.FirstChild.Attributes.FirstOrDefault(x => x.Name == "href").Value;
                    var BlogBody = BlogNodes[2].ChildNodes.Where(x => x.Name == "div" || x.Name == "p").ToList();
                    List<HtmlNode> BlogMeta = BlogBody[0].ChildNodes.Where(x => x.Name == "a").ToList();
                    string BlogAuthor = BlogMeta[0].InnerText;
                    string BlogCategory = BlogMeta[1].InnerText;
                    string BlogText = WebUtility.HtmlDecode(BlogBody[1].InnerText).Replace(",...  read more.", "");
                    DateTime BlogDate = Convert.ToDateTime(BlogBody[2].ChildNodes
                        .FirstOrDefault(x => x.Name == "ul").ChildNodes
                        .FirstOrDefault(x => x.Name == "li").InnerText);
                    var z = 0;
                    Blog newBlog = new Blog();
                    newBlog.Title = BlogName;
                    newBlog.Author = BlogAuthor;
                    newBlog.Category = BlogCategory;
                    newBlog.Text = BlogText;
                    newBlog.Publishdate = BlogDate;
                    newBlog.Image = BlogImage;
                    newBlog.Url = BlogUrl;
                    var exists = _context.Blog.FirstOrDefault(x => x.Title == BlogName);
                    if (exists == null)
                    {
                        _context.Blog.Add(newBlog);
                    }
                }
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return _context.Blog;
        }

        


        // GET: api/Blogs
        [HttpGet]
        public IEnumerable<Blog> GetAgency()
        {
            return _context.Blog.OrderBy(x => x.Title);
        }


        // GET: api/Agencies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agency = await _context.Blog.SingleOrDefaultAsync(m => m.Id == id);

            if (agency == null)
            {
                return NotFound();
            }

            return Ok(agency);
        }

        
    }
}