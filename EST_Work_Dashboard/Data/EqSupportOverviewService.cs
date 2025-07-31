using EST_Work_Dashboard.Models;
using Microsoft.Data.SqlClient;

namespace EST_Work_Dashboard.Data
{
    public class EqSupportOverviewService
    {        
        private readonly string _connectionString;

        public EqSupportOverviewService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<EqSupportOverviewModel>> GetAllAsync()
        {
            var list = new List<EqSupportOverviewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query = "SELECT * FROM EST_EqSupportOverview ORDER BY Down_Date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new EqSupportOverviewModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Classification = reader["Classification"].ToString(),
                            CP = reader["CP"].ToString(),
                            Manager = reader["Manager"].ToString(),
                            Process = reader["Process"].ToString(),
                            Equipment = reader["Equipment"].ToString(),
                            EquipmentNo = reader["EquipmentNo"].ToString(),
                            Down_Reason = reader["Down_Reason"].ToString(),
                            Actions = reader["Actions"].ToString(),
                            Status = reader["Status"].ToString(),
                            Down_Date = reader.GetDateTime(reader.GetOrdinal("Down_Date")),
                            Recovery_Date = reader.GetDateTime(reader.GetOrdinal("Recovery_Date")),
                            Down_Time = reader["Down_Time"].ToString(),
                            Technician = reader["Technician"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public async Task InsertAsync(EqSupportOverviewModel item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = @"INSERT INTO EST_EqSupportOverview
                                 (Classification, CP, Manager, Process, Equipment, EquipmentNo, Down_Reason, Actions, Status, Down_Date, Recovery_Date, Down_Time, Technician)
                                 VALUES
                                 (@Classification, @CP, @Manager, @Process, @Equipment, @EquipmentNo, @Down_Reason, @Actions, @Status, @Down_Date, @Recovery_Date, @Down_Time, @Technician)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Classification", item.Classification ?? "");
                    cmd.Parameters.AddWithValue("@CP", item.CP ?? "");
                    cmd.Parameters.AddWithValue("@Manager", item.Manager ?? "");
                    cmd.Parameters.AddWithValue("@Process", item.Process ?? "");
                    cmd.Parameters.AddWithValue("@Equipment", item.Equipment ?? "");
                    cmd.Parameters.AddWithValue("@EquipmentNo", item.EquipmentNo ?? "");
                    cmd.Parameters.AddWithValue("@Down_Reason", item.Down_Reason ?? "");
                    cmd.Parameters.AddWithValue("@Actions", item.Actions ?? "");
                    cmd.Parameters.AddWithValue("@Status", item.Status ?? "");
                    cmd.Parameters.AddWithValue("@Down_Date", item.Down_Date);
                    cmd.Parameters.AddWithValue("@Recovery_Date", item.Recovery_Date);
                    cmd.Parameters.AddWithValue("@Down_Time", item.Down_Time ?? "");
                    cmd.Parameters.AddWithValue("@Technician", item.Technician ?? "");

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<EqSupportOverviewModel?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM EST_EqSupportOverview WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new EqSupportOverviewModel
                {
                    Id = id,
                    Classification = reader["Classification"].ToString(),                    
                    CP = reader["CP"].ToString(),
                    Manager = reader["Manager"].ToString(),
                    Process = reader["Process"].ToString(),
                    Equipment = reader["Equipment"].ToString(),
                    EquipmentNo = reader["EquipmentNo"].ToString(),
                    Down_Reason = reader["Down_Reason"].ToString(),
                    Actions = reader["Actions"].ToString(),
                    Status = reader["Status"].ToString(),
                    Down_Date = reader["Down_Date"] as DateTime?,
                    Recovery_Date = reader["Recovery_Date"] as DateTime?,
                    Down_Time = reader["Status"].ToString(),
                    Technician = reader["Status"].ToString()                    
                };
            }

            return null;
        }

        public async Task UpdateAsync(EqSupportOverviewModel item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"UPDATE EST_EqSupportOverview SET
                                        Classification = @Classification,
                                        CP = @CP, Manager = @Manager, 
                                        Process = @Process,
                                        Equipment = @Equipment, EquipmentNo = @EquipmentNo,
                                        Down_Reason = @Down_Reason, Actions = @Actions, Status = @Status,
                                        Down_Date = @Down_Date, Recovery_Date = @Recovery_Date, Down_Time = @Down_Time,
                                        Technician = @Technician                                        
                                 WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", item.Id);
                    cmd.Parameters.AddWithValue("@Classification", item.Classification ?? "");                    
                    cmd.Parameters.AddWithValue("@CP", item.CP ?? "");
                    cmd.Parameters.AddWithValue("@Manager", item.Manager ?? "");
                    cmd.Parameters.AddWithValue("@Process", item.Process ?? "");
                    cmd.Parameters.AddWithValue("@Equipment", item.Equipment ?? "");
                    cmd.Parameters.AddWithValue("@EquipmentNo", item.EquipmentNo ?? "");
                    cmd.Parameters.AddWithValue("@Down_Reason", item.Down_Reason ?? "");                    
                    cmd.Parameters.AddWithValue("@Actions", item.Actions ?? "");                    
                    cmd.Parameters.AddWithValue("@Status", item.Status ?? "");
                    cmd.Parameters.AddWithValue("@Down_Date", item.Down_Date);
                    cmd.Parameters.AddWithValue("@Recovery_Date", item.Recovery_Date);
                    cmd.Parameters.AddWithValue("@Down_Time", item.Down_Time ?? "");
                    cmd.Parameters.AddWithValue("@Technician", item.Technician ?? "");                    

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "DELETE FROM EST_EqSupportOverview WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}