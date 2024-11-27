
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
namespace MSI.Models
{

    public class DataManagementcs
    {
        private readonly string ConnectionString;

        public DataManagementcs(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("conn");
        }
        public int uploaddatainserted(UploadFileDetails objFileDetails)
        {
            int result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("pro_InsertUploadVideoDetails", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@systemid", objFileDetails.systemid);
                        //cmd.Parameters.AddWithValue("@systemName", objFileDetails.systemname);
                        cmd.Parameters.AddWithValue("@uploadVideoPath", objFileDetails.filepath);
                        cmd.Parameters.AddWithValue("@uploadDateTime", objFileDetails.uploaddatetime);
                        cmd.Parameters.AddWithValue("@userid", objFileDetails.uploadEmployee);
                        conn.Open();
                        result = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        public string GetDomainSid()
        {
            try
            {
                // Get the domain name of the current machine
                string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                if (string.IsNullOrEmpty(domainName))
                {
                    return "This machine is not part of a domain.";
                }

                // Use DirectoryServices to find the domain SID
                System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry($"LDAP://{domainName}");
                byte[] sidBytes = (byte[])entry.Properties["objectSid"].Value;
                SecurityIdentifier sid = new SecurityIdentifier(sidBytes, 0);
                return sid.Value;
            }
            catch (Exception ex)
            {
                return $"An error occurred while retrieving the domain SID: {ex.Message}";
            }
        }

        public List<string> GetAllConnectedSystemNames()
        {
            var computerNames = new List<string>();

            try
            {
                // Get the domain context
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    using (var searcher = new PrincipalSearcher(new ComputerPrincipal(context)))
                    {
                        foreach (var result in searcher.FindAll())
                        {
                            using (var directoryEntry = result.GetUnderlyingObject() as DirectoryEntry)
                            {
                                if (directoryEntry != null)
                                {
                                    computerNames.Add(directoryEntry.Properties["name"].Value.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                computerNames.Add($"An error occurred: {ex.Message}");
            }

            return computerNames;
        }

        public List<SelectListItem> getSystemNames()
        {
            var list = new List<SelectListItem>();
            try
            {
                DataTable dtGetValue = new DataTable();

                using (SqlConnection conGetValue = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmdSetValue = new SqlCommand("pro_getSystemName", conGetValue))
                    {
                        cmdSetValue.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter daGetValue = new SqlDataAdapter(cmdSetValue))
                        {
                            daGetValue.Fill(dtGetValue);
                            if (dtGetValue.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtGetValue.Rows)
                                {
                                    list.Add(new SelectListItem { Value = row["system_id"].ToString(), Text = row["system_name"].ToString() });
                                }
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        public List<FileMappingDetails> getFileMappingDetails()
        {
            var lstFileMapping = new List<FileMappingDetails>();
            FileMappingDetails objFileMapping;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("pro_getFilemappingDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dataTable = new DataTable())
                            {
                                da.Fill(dataTable);
                                if (dataTable.Rows.Count > 0)
                                {
                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        objFileMapping = new FileMappingDetails();
                                        objFileMapping.systemid = Convert.ToInt32(row["systemid"].ToString());
                                        objFileMapping.systemname = row["system_name"].ToString();
                                        objFileMapping.filepath = row["File_Path"].ToString();
                                        lstFileMapping.Add(objFileMapping);
                                    }
                                }
                            }

                        }
                    }
                }
                return lstFileMapping;
            }

            catch (Exception ex)
            {
                return lstFileMapping;
            }
        }

        public int deleteFileMapping(int fileMappingId)
        {
            int resultDelete = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("pro_deleteFileMapping", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@systemid", fileMappingId);
                        con.Open();
                        resultDelete = command.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return resultDelete;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
