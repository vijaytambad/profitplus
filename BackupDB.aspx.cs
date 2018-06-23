using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.IO;

public partial class Backup_BackupDB : System.Web.UI.Page
{
    private string _ConnectionString = ConfigurationManager.ConnectionStrings["connectionString"].ToString();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            FillDatabases();
            ReadBackupFiles();
        }
    }

    private void FillDatabases()
    {
        try
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = _ConnectionString;
            sqlConnection.Open();
            string sqlQuery = "SELECT * FROM sys.databases";
            SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlCommand.CommandType = CommandType.Text;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();
            sqlDataAdapter.Fill(dataSet);
            ddlDatabases.DataSource = dataSet.Tables[0];
            ddlDatabases.DataTextField = "name";
            ddlDatabases.DataValueField = "database_id";
            ddlDatabases.DataBind();
        }
        catch (SqlException sqlException)
        {
            lblMessage.Text = sqlException.Message.ToString();
        }
        catch (Exception exception)
        {
            lblMessage.Text = exception.Message.ToString();
        }
    }

    protected void btnBackup_Click(object sender, EventArgs e)
    {
        try
        {
            string _DatabaseName = ddlDatabases.SelectedItem.Text.ToString();
            string _BackupName = _DatabaseName + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + ".bak";

            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = _ConnectionString;
            sqlConnection.Open();
            string sqlQuery = "BACKUP DATABASE " + _DatabaseName + " TO DISK = 'C:\\SQLServerBackups\\" + _BackupName + "' WITH FORMAT, MEDIANAME = 'Z_SQLServerBackups', NAME = '" + _BackupName + "';";
            SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlCommand.CommandType = CommandType.Text;
            int iRows = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
            lblMessage.Text = "The " + _DatabaseName + " database Backup with the name " + _BackupName + " successfully...";
            ReadBackupFiles();
        }
        catch (SqlException sqlException)
        {
            lblMessage.Text = sqlException.Message.ToString();
        }
        catch (Exception exception)
        {
            lblMessage.Text = exception.Message.ToString();
        }
    }

    private void ReadBackupFiles()
    {
        try
        {
           if(!Directory.Exists(@"c:\SQLServerBackups\"))
           {
               Directory.CreateDirectory(@"c:\SQLServerBackups\");
           }

            string[] files = Directory.GetFiles(@"c:\SQLServerBackups\", "*.bak");
            lstBackupfiles.DataSource = files;
            lstBackupfiles.DataBind();
            lstBackupfiles.SelectedIndex = 0;
        }
        catch (Exception exception)
        {
            lblMessage.Text = exception.Message.ToString();
        }
    }


    protected void btnRestore_Click(object sender, EventArgs e)
    {
        try
        {
            if (lstBackupfiles.SelectedIndex > 0)
            {
                string _DatabaseName = ddlDatabases.SelectedItem.Text.ToString();
                string _BackupName = lstBackupfiles.SelectedItem.Text.ToString();

                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = _ConnectionString;
                sqlConnection.Open();
                string sqlQuery = "RESTORE DATABASE " + _DatabaseName + " FROM DISK ='" + _BackupName + "'";
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                int iRows = sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
                lblMessage.Text = "The " + _DatabaseName + " database restored with the name " + _BackupName + " successfully...";
            }
        }
        catch (SqlException sqlException)
        {
            lblMessage.Text = sqlException.Message.ToString();
        }
        catch (Exception exception)
        {
            lblMessage.Text = exception.Message.ToString();
        }
    }
}
