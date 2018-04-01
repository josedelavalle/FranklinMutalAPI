<img src="http://me.josedelavalle.com/images/JoseDeLavalleLogo.png" />
Web Scraper - Data Importer - API for Franklin Mutual Sample App
<a target="_blank" href="https://fmi.josedelavalle.com">fmi.josedelavalle.com</a>

This is a .NET Core 2 project that utilizes Entity Framework to connect to a SQL Server Database (hosted on Azure).  Its function is to scrape <a target="_blank" href="https://www.fmiweb.com">Franklin Mutual Insurance's</a> website for publicly available agency data and blog entries.  Data is then stored in a database for accompanying API, to dispense JSON upon request.

There is an accompanying Angular 5 app that consumes and displays this data available <a target="_blank" href="http://github.com/josedelavalle/FranklinMutualApp">here</a>.
