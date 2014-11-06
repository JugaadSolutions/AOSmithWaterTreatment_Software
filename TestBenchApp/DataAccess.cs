using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Sql;
using System.ComponentModel;
using System.Collections.ObjectModel;
using TestBenchApp.Entity;


namespace TestBenchApp
{
    public class DataAccess
    {
        public enum TransactionStatus
        {
            NONE = 0, OPEN = 1, INPROCESS = 2,
            COMPLETE = 3, TIMEOUT = 4
        };
        public static String conStr;

        #region CONSTRUCTORS
        public DataAccess()
        {
            if( conStr ==String.Empty)
            conStr = ConfigurationSettings.AppSettings["DBConStr"];
        }

        public DataAccess(String conStr)
        {
            DataAccess.conStr = conStr;
        }
        #endregion

        #region LINEMANAGEMENT
        public lineCollection getLines()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            lineCollection lines = new lineCollection();

            String qry = String.Empty;
            qry = @"SELECT id , description FROM lines ORDER BY id";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                lines.add(new line((int)dt.Rows[i]["id"], (string)dt.Rows[i]["description"]));
            }

            con.Close();
            con.Dispose();
            return lines;
        }


        public void addLine(int ID, string description)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into lines(id ,description) values({0},'{1}')";
            qry = String.Format(qry, ID, description);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public void deleteLine(int ID)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from lines where id={0}";
            qry = String.Format(qry, ID);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void updateLine(int currentID, int modifiedID, string description)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"update lines set id = {0} , description ='{1}' where id ={2}";
            qry = String.Format(qry, modifiedID, description, currentID);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


     
        public StationCollection getStations(int line)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            StationCollection stations = new StationCollection();

            String qry = String.Empty;
            qry = @"SELECT id , description FROM stations where line={0} ORDER by id";

            qry = String.Format(qry, line);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                stations.Add(new Station(line,(int)dt.Rows[i]["id"], (string)dt.Rows[i]["description"]));
            }

            con.Close();
            con.Dispose();
            return stations;
        }


        public void removeStation(int line, int station)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from stations where line = {0} and id= {1}";
            qry = String.Format(qry, line, station);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            qry = @"delete from class where line ={0} and id = {0}";
            qry = String.Format(qry, line, station);
            cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void addStation(int line, int ID, string description)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into stations(line , id , description)
                    values({0},{1},'{2}')";
            qry = String.Format(qry, line, ID, description);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public ClassCollection getClass(int line, int station, int department)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            ClassCollection Class = new ClassCollection();

            String qry = String.Empty;
            qry = @"SELECT id , description FROM class where line={0} and station = {1} and
                department={2} ORDER by id";

            qry = String.Format(qry, line,station , department);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Class.Add(new Class(department, (int)dt.Rows[i]["id"], (string)dt.Rows[i]["description"]));
            }

            con.Close();
            con.Dispose();
            return Class;
        }

        public void addClass(int line, int station, int department, int code, string description)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into class(line ,station, department , id, description)
                    values({0},{1},{2},{3},'{4}')";
            qry = String.Format(qry, line, station, department, code, description);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void removeClass( int line , int station , int department , int code)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from class where line={0} and station={1}
                    and department={2} and  id = {3}";
            qry = String.Format(qry, line, station, department, code);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        #endregion

        #region USERS
        public Users GetUsers()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            Users users = new Users();

            String qry = String.Empty;
            qry = @"SELECT * FROM Users ";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                users.Add(new User((string)dt.Rows[i]["Name"], (string)dt.Rows[i]["password"]));
            }

            con.Close();
            con.Dispose();
            return users;
        }

        public bool Login(String UserName, String Password)
        {
            SqlConnection cn;
            SqlCommand cmd;

            cn = new SqlConnection(conStr);
            String query = @"SELECT [Password] FROM [Users] WHERE  Name='{0}'";
            query = String.Format(query, UserName);

            cmd = new SqlCommand(query, cn);
            cn.Open();
            cmd.Connection = cn;
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cn.Close();
            if (dt.Rows.Count > 0)
            {
                if (((string)dt.Rows[0]["Password"]) == Password)
                    return true;


            }

            return false;

        }


        #endregion

        #region MODEL

        public List<Model> GetModels()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT * from  Models";


            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            List<Model> models = new List<Model>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                models.Add(new Model { Name = (string)dt.Rows[i]["Name"], Number = (string)dt.Rows[i]["Number"] });
            }


            con.Close();
            con.Dispose();

            return models;
        }

        #endregion

        #region PLAN
        public List<Plan> GetPlans()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT * from  Plans inner join Models where timestamp > '{0}'" ;

            qry = String.Format(qry, DateTime.Now.ToString("yyyy-MM-dd"));


            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            List<Plan> plans = new List<Plan>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                plans.Add(new Plan
                {
                    slNumber = (int)dt.Rows[i]["slNo"],
                    ModelNumber = (string)dt.Rows[i]["Model"],
                    Actual = (int)dt.Rows[i]["Actual"],
                    Quantity = (int)dt.Rows[i]["Quantity"],
                    BSerialNo = (int)dt.Rows[i]["BSerial"], 
                    FSerialNo = (int)dt.Rows[i]["FSerial"],
                    CombinationSerialNo = (int)dt.Rows[i]["CombinationSerial"],
                    Status = (bool)dt.Rows[i]["Status"],
                    ModelName = (String)dt.Rows[i]["Name"]
                });
            }


            con.Close();
            con.Dispose();

            return plans;
        }

        #endregion


        #region TRACKING
        public void InsertUnit(String model, Model.Type type, int serialNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"insert into [Unit](Model,SerialNo,Type,Status,PlanDate,Barcode)
                        values('{0}',{1},{2},'{3}','{4}','{5}')";

            DateTime ts = DateTime.Now;
            String barcode = model + ((type == Model.Type.BODY )?"A":"") + ts.ToString("yy-MM-dd") + serialNo.ToString("D4");

            qry = String.Format(qry, model, serialNo, type, "NG", ts.ToString("yyyy-MM-dd HH:mm:ss"), barcode);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();


            con.Close();
            con.Dispose();
        }
        public void UpdateUnit(String barcode)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"update [Unit] set status = 'OK' , timestamp = '{0}' where barcode='{1}'";
            qry = String.Format(qry, DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"), barcode);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        #endregion


        public Queue<int> getDeviceQ()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT ID from  deviceAssociation" ;

           
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            Queue<int> deviceQ = new Queue<int>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                deviceQ.Enqueue((int)dt.Rows[i]["ID"]);
            }


            con.Close();
            con.Dispose();

            return deviceQ;
        }


        public DeviceAssociation getDeviceAssociation(int id)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select deviceAssociation.ID ,
                    deviceAssociation.Line ,
                    deviceAssociation.Station,
                    lines.header as Header,
                    lines.description as LineName , stations.description as StationName from deviceAssociation 
                    inner join lines on lines.id = deviceAssociation.line
                    left outer join stations on stations.id = deviceAssociation.station and lines.id = stations.line
                    where deviceAssociation.id = {0} ";
            
            qry = String.Format(qry, id);
            

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            if (dt.Rows.Count == 0)
                return null;
            DeviceAssociation dA= null;
            if ((int)dt.Rows[0]["Station"] != 0)
            {
                dA = new DeviceAssociation((int)dt.Rows[0]["ID"],
                                                    (String)dt.Rows[0]["Header"],
                                                    (int)dt.Rows[0]["Line"],
                                                    (String)dt.Rows[0]["LineName"],
                                                    (int)dt.Rows[0]["Station"],
                                                    (String)dt.Rows[0]["StationName"]);
            }

            else
            {
                dA = new DeviceAssociation((int)dt.Rows[0]["ID"],
                                                    (String)dt.Rows[0]["Header"],
                                                      (int)dt.Rows[0]["Line"],
                                                    (String)dt.Rows[0]["LineName"]
                                                    );
            }


            con.Close();
            con.Dispose();

            return dA;
        }

        public int addContact(string number, string name)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into contacts(number , name)
                    values('{0}','{1}')";
            qry = String.Format(qry, number, name);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            qry = String.Empty;
            qry = @"select slNo from contacts where number = '{0}' and name ='{1}'";
            qry = String.Format(qry, number, name);
            cmd = new SqlCommand(qry, con);

            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();




            con.Close();
            con.Dispose();

            return (int)dt.Rows[0][0];
        }


        public void removeContact(int slNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from contacts where slNo = {0}";
            qry = String.Format(qry, slNo);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            qry = @"delete from sms_entity_map where contact= {0}";
            qry = String.Format(qry, slNo);
            cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public void addToEscalationList(int id, int line, int department, int level)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into sms_entity_map(contact ,line , department,level)
                    values({0},{1},{2},{3})";
            qry = String.Format(qry, id, line, department, level);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void removeContactFromEscalationList(int id, int line, int department, int level)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from sms_entity_map where contact={0} and line={1} and department={2}and level={3}";
            qry = String.Format(qry, id, line, department, level);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public DataTable getStationsInfo(String stations)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT stations.id , stations.description AS STATION_DESCRIPTION 
                    FROM stations ";
            String qry1 = String.Empty;
            if (stations != String.Empty)
            {
                qry1 = " where id in ({0})";
                qry1 = String.Format(qry1, stations);
            }

            qry = String.Format(qry, stations);
            qry += qry1;

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            con.Close();
            con.Dispose();

            return dt;
        }

        public DataTable getDepartmentInfo(String departments)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select * from departments";
            if (departments != String.Empty)
            {
                qry += " where id in ({0})";
                qry = String.Format(qry, departments);
            }

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();
            return dt;
        }


        public int insertIssueRecord(int line, int station, int department, String data, String message,int device)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;

            SqlCommand cmd = new SqlCommand(qry, con);
            DataTable dt = new DataTable();

            cmd = new SqlCommand("insertIssueRecord", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@line", line);
            cmd.Parameters.AddWithValue("@station", station);
            cmd.Parameters.AddWithValue("@department", (int)department);
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@message", message);
            cmd.Parameters.AddWithValue("@device", device);
            SqlDataReader dr = cmd.ExecuteReader();
            dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            int slNo = (int)dt.Rows[0][0];
            return slNo;

        }


       


        public int findIssueRecord(int line, int station, int department, String data)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select slNo from issues where line = {0} and
                    station = {1} and department = {2} and data = '{3}' 
                    and status <> 'resolved'";
            qry = String.Format(qry,line, station, department, data);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();

            if ((dt.Rows.Count <= 0) || (dt.Rows.Count > 1))
                return -1;

            else return (int)dt.Rows[0][0];
        }

        public void updateIssueStatus(int slNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"update issues set status = 'resolved' , timestamp = '{0}' where slNo={1}";
            qry = String.Format(qry, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), slNo);
            SqlCommand cmd = new SqlCommand(qry, con);
             
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public string getIssueStatus(int slNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select status from issues where slNo = {0}";
            qry = String.Format(qry, slNo);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();

            if (dt.Rows.Count == 1)
                return (String)dt.Rows[0][0];
            else return String.Empty;
        }

        public void updateIssueStatus(int slNo, String status)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"update issues set status = '{0}' , timestamp = '{1}' where slNo={2}";
            qry = String.Format(qry, status, DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"), slNo);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public void updateIssueStatus(DataTable dt)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            for(int i = 0 ;i < dt.Rows.Count;i++)

            {
                String qry = String.Empty;
                qry = @"update issues set status = 'resolved' , timestamp = '{0}' where slNo={1}";
                qry = String.Format(qry, DateTime.Now,(int) dt.Rows[i]["SlNo"]);
                SqlCommand cmd = new SqlCommand(qry, con);
             
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            con.Close();
            con.Dispose();
        }




        public int getActualQuantity(int line, int shift)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select actualQuantity from ProductionQuantity where line = {0} and shift = {1}";
            qry = String.Format(qry, line, shift);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();

            if (dt.Rows.Count == 1)
                return (int)dt.Rows[0]["actualQuantity"];
            else return 0;
        }

        public void updateActualQuantity(int line, int quantity)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry = String.Empty;
            qry = @"insert into actual(line,quantity,timestamp)
                    values({0},{1},'{2}')";
            qry = String.Format(qry,line,quantity, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }


        public void updateTargetQuantity(int line, int shift,int session, int quantity)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into target(line,shift,session,quantity,timestamp)
                    values({0},{1},{2},{3},'{4}')";
            qry = String.Format(qry, line, shift, session, quantity, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }


        

        public DataTable getCommands()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = @"select * from Command where [status] = {0} and request_date='{1}'";

            qry = String.Format(qry, (int)TransactionStatus.OPEN, DateTime.Today.ToString("yyyy-MM-dd"));

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return dt;
        }


        public bool updateCommandStatus(int id, TransactionStatus status)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = @"update Command set [status] = {0} where id = {1}";
            qry = String.Format(qry, (int)status, id);


            SqlCommand cmd = new SqlCommand(qry, con);
            try
            {
                result = cmd.ExecuteNonQuery();
            }
            catch (SqlException s)
            {
                result = 0;
            }
            return (result == 1);
        }

        public void updateIssueMarquee()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            string issueMarquee = String.Empty;
            String qry = @"select message from issues where [status] = 'raised' and DATEDIFF(hh,timestamp,GETDATE())<=24";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                issueMarquee += (String)dt.Rows[i]["message"] + "; ";
            }
            qry = @"update config set value ='{0}' where [key]='issueMarquee'";
            qry = String.Format(qry, issueMarquee);

            cmd = new SqlCommand(qry, con);
            cmd.ExecuteNonQuery();
        }

        public int insertIntoTL(int oobid, String command, int status)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"insert into to_TL(oobid ,command , status ,timestamp)
                    values({0},'{1}',{2},'{3}')";
            qry = String.Format(qry, oobid, command, status, DateTime.Now);
            SqlCommand cmd = new SqlCommand(qry, con);

            con.Close();
            con.Dispose();

            return cmd.ExecuteNonQuery();
        }


        public DataTable checkFromTL()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"Select * from from_TL where status = 1";
            SqlCommand cmd1 = new SqlCommand(qry, con);
            SqlDataReader dr1 = cmd1.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr1);
            dr1.Close();
            cmd1.Dispose();


            con.Close();
            con.Dispose();

            return dt;


        }

        public int updateFromTL(int slno, int status)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = @"update from_TL set status = {0} , timestamp= '{1}' where slno = {2}";
            qry = String.Format(qry, status, DateTime.Now, slno);
            SqlCommand cmd3 = new SqlCommand(qry, con);


            int result = cmd3.ExecuteNonQuery();
            cmd3.Dispose();


            con.Close();
            con.Dispose();

            return result;
        }







        #region ManageTab

        public DataTable getReceivers(int line,int department , int escalationLevel)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select receiver as Receivers from sms_receiver 
                inner join sms_entity_map on sms_receiver.slNo = sms_entity_map.receiver_id where entity_1_id = {0}
                and entity_2_id = {1} and parameter_1 = {2}";

            qry = String.Format(qry, department,line, escalationLevel);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return dt;
        }


        public Dictionary<String, int> loadReceiverList()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from sms_receiver";

            Dictionary<String, int> receiverList = new Dictionary<string, int>();

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                receiverList.Add((String)dt.Rows[i]["receiver"], (int)dt.Rows[i]["slNo"]);
            }

            return receiverList;
        }

        public int getReceiverSlNo(String receiver)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from sms_receiver where receiver = '{0}'";
            qry = String.Format(qry, receiver);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            int slNo = (dt.Rows.Count > 0) ? (int)(dt.Rows[0]["slNo"]) : -1;
            return slNo;
        }
        public String getMessage(int department)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select message as Message from departments
                 where id = {0}";

            qry = String.Format(qry, department);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return (String)dt.Rows[0][0];

        }


        public bool updateMessage(String message, int department)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = @"update departments set message = '{0}' where [id] = {1}";

            qry = String.Format(qry, message, department);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);//If result=1, means update is successful, otherwise not

        }


        public void addReceiver(String receiver,int line, int department, int slNo,int escalationLevel)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = String.Empty;
            SqlCommand cmd = null;


            if (slNo == -1)
            {
                qry = @"insert into sms_receiver(receiver) values('{0}')";
                qry = String.Format(qry, receiver);

                cmd = new SqlCommand(qry, con);
                result = cmd.ExecuteNonQuery();

                qry = @"select Count(*) from sms_receiver";
                cmd = new SqlCommand(qry, con);

                SqlDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dr.Close();

                slNo = (int)dt.Rows[0][0];
            }

            qry = @"insert into sms_entity_map(receiver_id , entity_1_id,entity_2_id,parameter_1) values({0},{1},{2},{3})";
            qry = String.Format(qry, slNo, department,line, escalationLevel);

            cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();
        }

        public bool removeReceiver(int slNo,int line, int department,int escalationLevel)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = @"delete from sms_entity_map where receiver_id = {0} and entity_1_id = {1} 
            and entity_2_id = {2} and parameter_1 = {3} ";

            qry = String.Format(qry, slNo, department,line,escalationLevel);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);//If result=1, means update is successful, otherwise not
        }

        public bool insertSmsTrigger(string receiver, string message, int status,String timestamp)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            int result = 0;
            String qry = @"insert into sms_trigger(receiver , message , status,timestamp) values('{0}','{1}',{2},'{3}') ";

            qry = String.Format(qry, receiver, message, status,timestamp);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);//If result=1, means update is successful, otherwise not

        }




        #endregion

        #region ReportTab

        public DataTable GetReportData(DateTime from , DateTime to)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            to = to.AddDays(1);
            String qry = @"select Substring(Convert(nvarchar,raised.timestamp,0),0,12) as DATE, 
                        
                        stations.description as LINE , 
                        departments.description as ISSUE , 
                        issues.data as DETAILS,
                        CONVERT(TIME(0), raised.timestamp,0) as RAISED , 
                        CONVERT(TIME(0), resolved.timestamp,0) as RESOLVED ,
                        CONVERT(Time(0) , resolved.timestamp - raised.timestamp , 0) as DOWNTIME 
                        from issues
                        inner join stations on stations.id = issues.station
                        inner join lines on lines.id = stations.line_id
                        inner join departments on issues.department = departments.id
                        inner join ( select issue_no, timestamp from issue_tracker where status = 'raised') as raised 
                        on raised.issue_no = issues.slNo

                        left outer join (select issue_no , timestamp from issue_tracker where status = 'resolved')
                        as resolved on resolved.issue_no = issues.slNo 
                        where raised.timestamp >= '{0}' and raised.timestamp <= '{1}'";

            qry = String.Format(qry , from.ToShortDateString(), to.ToShortDateString());

            SqlCommand cmd = new SqlCommand(qry,con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);

            dr.Close();
            
            return dt;


        }


        public DataTable GetOpenIssues()
        {
            SqlConnection localCon = new SqlConnection(conStr);
            String qry = @"select distinct Substring(Convert(nvarchar,issues.timestamp,0),0,12) as DATE, 
                        issues.slNo as SlNo,
                        issues.station as STATION_NO,
                        lines.description as LINE , 
                        stations.description as STATION_NAME,
                        departments.description as ISSUE , 
                        issues.data as DETAILS,
                        CONVERT(TIME(0), issues.timestamp,0) as RAISED 
                        from issues
                        LEFT OUTER JOIN stations on (stations.id = issues.station and stations.line = issues.line)
                        inner join lines on lines.id = issues.line
                        left outer join departments on issues.department = departments.id
                        where status='raised'";


            localCon.Open();

            SqlCommand cmd = new SqlCommand(qry, localCon);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);

            dr.Close();

            localCon.Close();
            localCon.Dispose();

            return dt;
        }





        #endregion


        ~DataAccess()
        {

        }

        public void close()
        {

        }

        internal void ChangePassword(User user)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = @"update [Users] set Password = '{0}'  where Name = '{1}'";
            qry = String.Format(qry, user.Password,user.Name);
            SqlCommand cmd3 = new SqlCommand(qry, con);


            int result = cmd3.ExecuteNonQuery();
            cmd3.Dispose();


            con.Close();
            con.Dispose();

            return;
        }

        internal void InsertPlan(Plan p)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"insert into [Plans] ( Model, Quantity, timestamp)
                    values('{0}', {1}, '{2}')";
            qry = String.Format(qry,p.ModelNumber, p.Quantity, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();

            
        }

        internal void UpdatePlan(Plan p)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"update [Plans] set Quantity={0} where SlNo = {1}";
            qry = String.Format(qry, p.Quantity, p.slNumber);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        internal void DeletePlan(Plan p)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"delete from [Plans] where slNo = {0}";
            qry = String.Format(qry, p.slNumber);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }
    }
}
