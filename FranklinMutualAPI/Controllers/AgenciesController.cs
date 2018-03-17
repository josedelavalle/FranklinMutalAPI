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

namespace FranklinMutualAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Agencies")]
    public class AgenciesController : Controller
    {
        private readonly FranklinMutualContext _context;

        HtmlWeb web = new HtmlWeb();

        public AgenciesController(FranklinMutualContext context)
        {
            _context = context;
        }


        [HttpGet("[action]")]
        public IEnumerable<Agency> ImportData()
        {
            // get first page with all counties listed and url's to drill down to
            try
            {

            
                string url = "https://www.fmiweb.com/site/locate-agent";
                HtmlAgilityPack.HtmlDocument doc = web.Load(url);

                // populate list of all counties
                IEnumerable<HtmlNode> countyList = doc.DocumentNode.SelectNodes("//li[@class='list-item']");

                // loop through all counties found
                foreach (HtmlNode county in countyList)
                {
                    // get node with needed data
                    HtmlNode CountyNode = county.ChildNodes.FirstOrDefault(c => c.Name == "a");

                    // get name of the county we are working on and url to navigate to to get agents for corresponding county
                    string CountyName = CountyNode.InnerText;
                    string CountyUrl = CountyNode.Attributes.FirstOrDefault(a => a.Name == "href").Value;

                    // load county specific url
                    doc = web.Load(CountyUrl);

                    // get list of all agents for county and parse into database
                    IEnumerable<HtmlNode> agentList = doc.DocumentNode.SelectSingleNode("//tbody").ChildNodes.Where(a => a.Name == "tr");
                    ProcessAgentList(agentList, CountyName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return _context.Agency;
        }

        public void ImportAgents()
        {
            try
            {
                string url = "";
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ProcessAgentList(IEnumerable<HtmlNode> agentList, string CountyName)
        {
            foreach (HtmlNode agent in agentList)
            {
                string AgentUrl = String.Empty;

                // get the table row for this agent so we can parse through the details
                List<HtmlNode> AgentDetails = agent.ChildNodes.Where(d => d.Name == "td").ToList();
                string AgentCity = AgentDetails[0].ChildNodes.FirstOrDefault(x => x.Name == "span").InnerText;
                HtmlNode AgentNameNode = AgentDetails[1].ChildNodes.FirstOrDefault(x => x.Name == "a");

                
                if (AgentNameNode != null)
                {
                    // some rows have an <a> tag to link to agency external websites
                    AgentUrl = AgentNameNode.Attributes.FirstOrDefault(x => x.Name == "href").Value;
                }
                else
                {
                    // and others don't
                    AgentNameNode = AgentDetails[1].ChildNodes.FirstOrDefault(x => x.Name == "span");
                }

                string AgentName = AgentNameNode.InnerText;

                // get only the first ten digits of phone number without dashes to store as little as possible in database
                string AgentPhone = AgentDetails[2].ChildNodes.FirstOrDefault(x => x.Name == "span").InnerText.Replace("-", "");

                // some nodes have more than one phone number - just get the first one
                if (AgentPhone.Length > 10) AgentPhone = AgentPhone.Substring(0, 10);

                // see if we have imported this in the past and if so skip
                Agency existing = _context.Agency.FirstOrDefault(x => x.Name == AgentName && x.City == AgentCity);
                if (existing == null)
                {
                    Agency a = new Agency()
                    {
                        Name = AgentName,
                        City = AgentCity,
                        State = "NJ",   // NJ Insurance Company
                        Url = AgentUrl,
                        Phone = AgentPhone,
                        County = CountyName
                    };
                    _context.Agency.Add(a);
                    _context.SaveChanges();
                }
            }
        }

        [HttpGet("[action]")]
        public IEnumerable<Agency> GeocodeAgencies()
        {
            try
            {
                var agencies = _context.Agency.Where(x => x.Address == "").ToList();
                foreach (var agency in agencies)
                {
                    string loc = $"{agency.Name} {agency.City} NJ";
                    var Geocoded = GeocodeLocation(loc);
                    if (Geocoded != null)
                    {
                        agency.Latitude = Geocoded.Item1;
                        agency.Longitude = Geocoded.Item2;
                        agency.Address = Geocoded.Item3;
                        agency.Zip = Geocoded.Item4;
                        _context.SaveChanges();
                    }
                    
                }
                
            }
            catch (Exception)
            {

                throw;
            }

            return _context.Agency;
        }


        // GET: api/Agencies
        [HttpGet]
        public IEnumerable<Agency> GetAgency()
        {
            return _context.Agency;
        }

        // GET: api/Agencies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgency([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agency = await _context.Agency.SingleOrDefaultAsync(m => m.Id == id);

            if (agency == null)
            {
                return NotFound();
            }

            return Ok(agency);
        }

        // PUT: api/Agencies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgency([FromRoute] int id, [FromBody] Agency agency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != agency.Id)
            {
                return BadRequest();
            }

            _context.Entry(agency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgencyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Agencies
        [HttpPost]
        public async Task<IActionResult> PostAgency([FromBody] Agency agency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Agency.Add(agency);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AgencyExists(agency.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAgency", new { id = agency.Id }, agency);
        }

        // DELETE: api/Agencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgency([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agency = await _context.Agency.SingleOrDefaultAsync(m => m.Id == id);
            if (agency == null)
            {
                return NotFound();
            }

            _context.Agency.Remove(agency);
            await _context.SaveChangesAsync();

            return Ok(agency);
        }

        private bool AgencyExists(int id)
        {
            return _context.Agency.Any(e => e.Id == id);
        }

        private static Tuple<string, string, string, string> GeocodeLocation(string loc)
        {
            try
            {
                string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyBPsDaLY2SWLxNqHNbZzjkMBgGVisLqFx8&address={0}&sensor=false", Uri.EscapeDataString(loc));

                WebRequest request = WebRequest.Create(requestUri);
                WebResponse response = request.GetResponse();
                XDocument xdoc = XDocument.Load(response.GetResponseStream());
                string address = String.Empty;
                string zip = String.Empty;
                XElement result = xdoc.Element("GeocodeResponse").Element("result");
                XElement locationElement = result.Element("geometry").Element("location");
                IEnumerable<XElement> addressElements = result.Elements("address_component");
                foreach (var el in addressElements)
                {
                    var type = el.Element("type").Value;
                    if (type == "street_number")
                    {
                        address = el.Element("long_name").Value + " ";
                    }
                    if (type == "route")
                    {
                        address += el.Element("long_name").Value;
                    }
                    if (type == "postal_code")
                    {
                        zip = el.Element("long_name").Value;
                    }
                }
                if (address == String.Empty)
                {
                    // if address didn't return with seperated parts
                    string formattedAddress = result.Element("formatted_address").Value;
                    address = formattedAddress.Substring(0, formattedAddress.IndexOf(","));
                }
                XElement lat = locationElement.Element("lat");
                XElement lng = locationElement.Element("lng");

                return Tuple.Create(lat.Value, lng.Value, address, zip);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}