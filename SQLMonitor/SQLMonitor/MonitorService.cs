using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Sql;
using System.Configuration;

namespace SQLMonitor
{
    public partial class MonitorService : ServiceBase
    {
        #region Variable declaration

        /// <summary>
        /// Option selected.
        /// </summary>
        private static string optionSelected;

        /// <summary>
        /// Sample connection string.
        /// </summary>
        private string sampleConnectionString;

        /// <summary>
        /// Notification query.
        /// </summary>
        private string notificationQuery;

        /// <summary>
        /// Notification stored procedure.
        /// </summary>
        private string notificationStoredProcedure;

        /// <summary>
        /// Instance of SQL dependency.
        /// </summary>
        private SqlDependency sampleSqlDependency;

        /// <summary>
        /// SQL command.
        /// </summary>
        private SqlCommand sampleSqlCommand;

        /// <summary>
        /// SQL connection
        /// </summary>
        private SqlConnection sampleSqlConnection;

        #endregion

        private const string _product = "product";
        private const string _product_properties = "product_properties";
        private const string _product_property = "product_property";
        private const string _property_name = "[property_name]=";
        private const string _value = "[value]=";
        private const string _amp = "&";

        private const string _name = "[name]=";
        private const string _description = "[description]=";
        private const string _price = "[price]=";
        private const string _display_price = "[display_price]=";
        private const string _shipping_category_id = "[shipping_category_id]=";
        private const string _sku = "[sku]=";
        private const string _cost_price = "[cost_price]=";
        private const string _total_on_hand = "[total_on_hand]=";
        private const string _saleprice = "saleprice";

        private const string _varietal = "Varietal";
        private const string _color = "Color";
        private const string _vintage = "Vintage";
        private const string _size = "Size";
        private const string _country = "Country";
        private const string _region = "Region";
        private const string _point = "Point";
        private const string _review = "Review";
        private const string _reviewer = "Reviewer";
        private const string _department = "Category";
        private const string _notes = "Notes";



        public MonitorService()
        {
            InitializeComponent();
           
        }

        protected override void OnStart(string[] args)
        {
            try
            {

            
            Library.WriteErrorLog("Monitor Service has started");
            sampleConnectionString = "Data Source=BC1REGISTER;Initial Catalog=mPower;User Id=sa;Password=p0w3r";
            //sampleConnectionString = "Data Source=Sweetu;Initial Catalog=mPower;Integrated Security=True";
            notificationQuery =
                "SELECT [SampleId],[SampleName],[SampleCategory],[SampleDateTime],[IsSampleProcessed] FROM [dbo].[SampleTable01];";
            notificationStoredProcedure = "uspGetStyle";
            Library.WriteErrorLog("local variables declared");

            SqlDependency.Stop(sampleConnectionString);
            Library.WriteErrorLog("stop1");
            SqlDependency.Stop(sampleConnectionString, "myNotifQueue");
            Library.WriteErrorLog("stop2");
            SqlDependency.Start(sampleConnectionString, "myNotifQueue");
            Library.WriteErrorLog("Stop3");

                ConfigureDependencyUsingStoreProcedureAndSpecificQueue();
            }
            catch (Exception ex)
            {
                
            Library.WriteErrorLog("exceptopion" + ex.Message  + ex.StackTrace);
            }
        }

        protected override void OnStop()
        {
            Library.WriteErrorLog("Monitor service has stopped");
        }

        public static int GetIdValue(string strtype)
        {
            int value1=0;
            try
            {
                var configFile = ConfigurationManager.AppSettings["configfile"].ToString();

                var json =
                    System.IO.File.ReadAllText(configFile);

                var objects = JArray.Parse(json); // parse as array  
                foreach (JObject root in objects)
                {
                    foreach (KeyValuePair<String, JToken> app in root)
                    {
                        var appName = app.Key;
                        //var description = (String)app.Value["LastReadProductId"];
                        value1 = Convert.ToInt32(app.Value);

                       
                    }
                }
                return value1;
            }

            catch (Exception ex)
            {
                {
                    Library.WriteErrorLog("Error in GetIdValue"+ex.Message);

                }
                throw;
            }
        }

        private const string URL = "http://ec2-54-84-50-223.compute-1.amazonaws.com";
        private const string token = "token=b37b2ebe0d476b2ef5918a03ad0792644c5f8fc5a305303";
        //private static string urlParameter = "/api/products/?token=b37b2ebe0d476b2ef5918a03ad0792644c5f8fc5a305303f?Name = 'testing2003', Price = 100, Shipping_category_id = 1";
        private static string urlParameter;

        static async Task RunAsync()
        {
            Library.WriteErrorLog("In RunAsync");
            int ProdId = 0;
            ProdId = GetIdValue("prod");
            Library.WriteErrorLog(ProdId.ToString());
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            // HTTP POST

            //client.DefaultRequestHeaders.Add(); = new AuthenticationHeaderValue("X-Spree-Token","b37b2ebe0d476b2ef5918a03ad0792644c5f8fc5a305303f");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Spree-Token", "b37b2ebe0d476b2ef5918a03ad0792644c5f8fc5a305303f");
            urlParameter = "api/products";
            //var gizmo = new Product() { Name = "Gizmo", Price = 100, Shipping_category_id = 1}f'GetStyleDetails'.;
            Library.WriteErrorLog("Before connection");

            var conn = ConfigurationManager.ConnectionStrings["connectionstring"].ToString();
            var prod = new Product();
            string urlParameterExtn = string.Empty;
            HttpResponseMessage response;

            Library.WriteErrorLog("connection" + conn.ToString());

            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("GetStyleDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StyleId", ProdId);
                    con.Open();
                    Library.WriteErrorLog("connection opened");
                    try
                    {

                        SqlDataReader myReader = cmd.ExecuteReader();
                        Library.WriteErrorLog("my reader");
                        if (myReader.HasRows)
                        {
                            Library.WriteErrorLog("has rows");
                            while (myReader.Read())
                            {
                                Library.WriteErrorLog("reader...");
                                prod.Name = myReader["name"].ToString();
                                prod.Price = Convert.ToDecimal(myReader["price"]);
                                prod.Shipping_category_id = 1;
                                prod.Skunumber = myReader["SKUNumber"].ToString();
                                Library.WriteErrorLog("reader...a");
                                prod.Notes = myReader["Notes"].ToString();
                                prod.Vintage = myReader["vintage"].ToString();
                                prod.Size = myReader["Size"].ToString();
                                prod.Points = Convert.ToInt32(myReader["Rating"]);
                                Library.WriteErrorLog("reader...b");
                                prod.Department = myReader["dept"].ToString();
                                prod.Varietal = myReader["varietal"].ToString();
                                prod.Color = myReader["color"].ToString();
                                prod.Country = myReader["country"].ToString();
                                prod.Region = myReader["region"].ToString();
                                prod.Review = myReader["review"].ToString();
                                prod.Reviewer = myReader["Reviewer"].ToString();
                                prod.SalePrice = Convert.ToDecimal(myReader["saleprice"]);

                                Library.WriteErrorLog("reader  1...");

                                urlParameterExtn = "?" + _product + _name + prod.Name + _amp + _product + _price +
                                                   prod.Price + _amp + _product + _shipping_category_id +
                                                   prod.Shipping_category_id +
                                                   _amp + _product + _sku + prod.Skunumber +
                                                   _amp + _product + _notes + prod.Notes +
                                                   _amp + _product + _vintage + prod.Vintage +
                                                   _amp + _product + _size + prod.Size +
                                                   _amp + _product + _point + prod.Points +
                                                   _amp + _product + _department + prod.Department +
                                                   _amp + _product + _varietal + prod.Varietal +
                                                   _amp + _product + _color + prod.Color +
                                                   _amp + _product + _country + prod.Country +
                                                   _amp + _product + _region + prod.Region +
                                                   _amp + _product + _review + prod.Review +
                                                   _amp + _product + _reviewer + prod.Reviewer +
                                                   _amp + _product + _saleprice + prod.SalePrice
                                    ;



                                urlParameter = urlParameter + urlParameterExtn;
                                Library.WriteErrorLog("URL Parameter   " + urlParameter);
                                response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);
                                Library.WriteErrorLog("Response" );
                                if (response.IsSuccessStatusCode)
                                {
                                    Library.WriteErrorLog("statuscode");
                                    string res = response.Content.ReadAsStringAsync().Result;
                                    JObject joResponse = JObject.Parse(res);
                                    int id = Convert.ToInt32(joResponse["id"].ToString());

                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _vintage + _amp + _product_property + _value + prod.Vintage;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);

                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _size + _amp + _product_property + _value + prod.Size;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);

                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _point + _amp + _product_property + _value + prod.Points;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);

                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _department + _amp + _product_property + _value + prod.Department;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);

                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _varietal + _amp + _product_property + _value + prod.Varietal;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);

                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _color + _amp + _product_property + _value + prod.Color;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);


                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _country + _amp + _product_property + _value + prod.Country;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);

                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _region + _amp + _product_property + _value + prod.Region;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);


                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _review + _amp + _product_property + _value + prod.Review;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);

                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _reviewer + _amp + _product_property + _value + prod.Reviewer;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);

                                    urlParameter = "api/products/" + id;
                                    urlParameterExtn = "/product_properties?" + _product_property + _property_name +
                                                       _saleprice + _amp + _product_property + _value + prod.SalePrice;

                                    urlParameter = urlParameter + urlParameterExtn;
                                    Library.WriteErrorLog("urlParameter ...." + urlParameter);
                                    response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);
                                    Library.WriteErrorLog("Response ...." + res.ToString());
                                    //Console.WriteLine(res);
                                    //Console.ReadKey();
                                }

                                urlParameter = "api/products";
                                urlParameterExtn = "";
                                //Uri gizmoUrl = response.Headers.Location;

                                //// HTTP PUT
                                //gizmo.Price = 80;   // Update price
                                //response = await client.PutAsJsonAsync(URL, gizmo);

                                //// HTTP DELETE
                                //response = await client.DeleteAsync(gizmoUrl);


                                Console.WriteLine(prod.Name + " " + prod.Price + " " + prod.Shipping_category_id);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        {
                            Library.WriteErrorLog("Error in RunAsync..." +ex.StackTrace.ToString());

                        }
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            //var prod = new Product();
            //prod.Name = "Test123890";
            //prod.Price = 100;
            //prod.Shipping_category_id = 1;

            //var urlParameterExtn = "?" + _product + _name + prod.Name + _amp + _product + _price + prod.Price + _amp + _product + _shipping_category_id +
            //            prod.Shipping_category_id ;



            //urlParameter = urlParameter + urlParameterExtn;
            //HttpResponseMessage response = await client.PostAsJsonAsync(urlParameter, urlParameterExtn);
            //if (response.IsSuccessStatusCode)
            //{
            //    string res = response.Content.ReadAsStringAsync().Result;
            //    JObject joResponse = JObject.Parse(res);
            //    int id = Convert.ToInt32(joResponse["id"].ToString());



            //    Console.WriteLine(res);
            //    Console.ReadKey();

            //    //Uri gizmoUrl = response.Headers.Location;

            //    //// HTTP PUT
            //    //gizmo.Price = 80;   // Update price
            //    //response = await client.PutAsJsonAsync(URL, gizmo);

            //    //// HTTP DELETE
            //    //response = await client.DeleteAsync(gizmoUrl);
            //}

            //////var prod = new Product() {Name = "testing2003", Price = 100, Shipping_category_id = 1};

            //var prod = new Product();
            //prod.Name = "testing123";
            //prod.Price = 100;
            //prod.Shipping_category_id = 1;

            //response = await client.PostAsJsonAsync(urlParameter,prod);

            ////var response = client.PostAsJsonAsync(urlParameter, prod).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine("Sucessful");
            //}

            //      /api/products?product[name]=Headphones&product[price]=100&product[shipping_category_id]=1
            //http://ec2-54-84-50-223.compute-1.amazonaws.com/api/products?product[name]=RedWine&product[price]=100&product[shipping_category_id]=1
            // http://ec2-54-84-50-223.compute-1.amazonaws.com/api/products?product[Name]=Test1&product[Price]=100&product[Shipping_category_id]=1
            //http://ec2-54-84-50-223.compute-1.amazonaws.com/api/products/35/product_properties?product_property[property_name]=varietal&product_property[value]=CABERNET`


        }

        public async void ConfigureDependencyUsingStoreProcedureAndSpecificQueue()
        {
            Library.WriteErrorLog("ConfigureDependencyUsingStoreProcedureAndSpecificQueue");
            try
            {
                if (null != sampleSqlDependency)
                {
                    //this.sampleSqlDependency.OnChange -= null;
                    sampleSqlDependency.OnChange -= null;
                }

                if (null != sampleSqlCommand)
                {
                    //this.sampleSqlCommand.Dispose();
                    sampleSqlCommand.Dispose();
                }

                if (null != sampleSqlConnection)
                {
                    //this.sampleSqlConnection.Dispose();
                    sampleSqlConnection.Dispose();
                }

                Library.WriteErrorLog("sampleSqlConnection" + sampleSqlConnection);

                //this.sampleSqlDependency = null;
                //this.sampleSqlCommand = null;
                //this.sampleSqlConnection = null;
                sampleSqlDependency = null;
                sampleSqlCommand = null;
                sampleSqlConnection = null;
                Library.WriteErrorLog("sampleSqlConnection" + sampleSqlConnection);

                //sampleConnectionString = ConfigurationManager.ConnectionStrings["SampleDbConnection"].ConnectionString;
                sampleConnectionString = "Data Source=BC1REGISTER;Initial Catalog=mPower;User Id=sa;Password=p0w3r";
                //sampleConnectionString = "Data Source=Sweetu;Initial Catalog=mPower;Integrated Security=True";
                notificationQuery =
                    "SELECT [SampleId],[SampleName],[SampleCategory],[SampleDateTime],[IsSampleProcessed] FROM [dbo].[SampleTable01];";
                notificationStoredProcedure = "uspGetStyle";

                //// Create connection.
                //this.sampleSqlConnection = new SqlConnection(this.sampleConnectionString);
                sampleSqlConnection = new SqlConnection(sampleConnectionString);

                Library.WriteErrorLog("sampleConnectionString" + sampleConnectionString);

                //// Create command.
                //this.sampleSqlCommand = new SqlCommand { Connection = this.sampleSqlConnection };
                //this.sampleSqlCommand.CommandType = CommandType.StoredProcedure;
                //this.sampleSqlCommand.CommandText = this.notificationStoredProcedure;
                //this.sampleSqlCommand.Notification = null;
                sampleSqlCommand = new SqlCommand {Connection = sampleSqlConnection};
                sampleSqlCommand.CommandType = CommandType.StoredProcedure;
                sampleSqlCommand.CommandText = notificationStoredProcedure;
                sampleSqlCommand.Notification = null;

                Library.WriteErrorLog("Dependency");
                //// Create Sql Dependency.
                sampleSqlDependency = new SqlDependency(sampleSqlCommand, "service=myNotifService;Local database=mPower",
                    432000);
                Library.WriteErrorLog("sampleSqlDependency");
                sampleSqlDependency.OnChange += SqlDependencyOnChange;
                Library.WriteErrorLog("OnChange");
                await sampleSqlCommand.Connection.OpenAsync();
                Library.WriteErrorLog("OpenAsync");
                await sampleSqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                Library.WriteErrorLog("CommandBehavior");

                if (null != sampleSqlCommand)
                {
                    sampleSqlCommand.Dispose();
                }

                if (null != sampleSqlConnection)
                {
                    sampleSqlConnection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Library.WriteErrorLog("Exception" +ex.Message);
            }
        }

        private void SqlDependencyOnChange(object sender, SqlNotificationEventArgs eventArgs)
        {
            Library.WriteErrorLog("Into SqlDependencyOnChange.....");

            try
            {

                if (eventArgs.Info == SqlNotificationInfo.Invalid)
                {
                    //Console.WriteLine("The above notification query is not valid.");

                    Library.WriteErrorLog("The above notification query is not valid.");
                }
                else
                {
                    //Console.WriteLine("Notification Info: " + eventArgs.Info);
                    //Console.WriteLine("Notification source: " + eventArgs.Source);
                    //Console.WriteLine("Notification type: " + eventArgs.Type);

                    //Library.WriteErrorLog("Monitor service has stopped" + eventArgs.Info);

                    Library.WriteErrorLog("Monitor service has stopped");
                    RunAsync().Wait();

                }
                ConfigureDependencyUsingStoreProcedureAndSpecificQueue();


            }
            catch (Exception ex)
            {

                Library.WriteErrorLog("SqlDependencyOnChange Error " + ex.Message);
            }

           
           
        }

    }
}
