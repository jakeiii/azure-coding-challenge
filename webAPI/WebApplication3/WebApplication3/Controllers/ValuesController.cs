using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;

namespace WebApplication3.Controllers
{
    public class ValuesController : ApiController
    {

        // GET api/values/5
        public string Get(string id)
        {
            string returnString = "";

            //BETWEEN sql clause for dates
            string betweenString = "";

            //split the input into usable parts
            List<string> myList = id.Split(',').ToList();

            //connection string to the azure sql database
            string someConnectionString = ("Server=tcp:jacob19.database.windows.net,1433;Initial Catalog=AdventureWork2017;Persist Security Info=False;User ID=JACOB19;Password=12BonJac66;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            SqlConnection conn = new SqlConnection(someConnectionString);

            conn.Open();

            if (myList.Count == 3) {
                betweenString = " and OrderDate between '" + myList[1] + "' and '" + myList[2] +"'";
            }

            //query to get average transaction from postal code that was passed in
            string sqlPos = "select avg(SubTotal) from Sales.SalesOrderHeader inner join Person.Address on Person.Address.AddressID = Sales.SalesOrderHeader.BillToAddressID where PostalCode = '" + myList[0] + "' and CreditCardApprovalCode is not null" + betweenString +";";
            SqlCommand command = new SqlCommand(sqlPos, conn);
            object reader = command.ExecuteScalar();
            returnString += reader.ToString();
            returnString += ",";
            //quert to get average transaction of territory from postal code
            string sqlTer = "select avg(SubTotal) from Sales.SalesOrderHeader inner join Person.Address on Person.Address.AddressID = Sales.SalesOrderHeader.BillToAddressID where TerritoryID = (select distinct SalesTerritory.TerritoryID from Person.Address inner join Person.StateProvince on Person.StateProvince.StateProvinceID = Person.Address.StateProvinceID inner join Sales.SalesTerritory on SalesTerritory.TerritoryID = Person.StateProvince.TerritoryID where PostalCode = '" + myList[0] + "'" + betweenString + ");";
            command = new SqlCommand(sqlTer, conn);
            reader = command.ExecuteScalar();
            returnString += reader.ToString();

            conn.Close();

            if(returnString == ",")
            {
                returnString = "0,0";
            }

            return returnString;

        }

       
    }
}
