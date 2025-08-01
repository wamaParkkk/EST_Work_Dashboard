using EST_Work_Dashboard.Models;
using Microsoft.Data.SqlClient;

namespace EST_Work_Dashboard.Data
{
    public class DailyRawDataService
    {
        private readonly string _connectionString;

        public DailyRawDataService(IConfiguration configuration)
        {
            // appsettings.json에서 연결 문자열을 가져옴
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // DailyRawData 테이블 전체 조회
        public async Task<List<DailyRawData>> GetAllAsync()
        {
            var list = new List<DailyRawData>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query = "SELECT Id, ww, StartDate, EndDate, CP, Manager, Classification, Site, Plant, Line, Process, Eq_Manufacturer, Model_Name, MC, Problem_Description, Actions, Cause, Status, TTL_Qty, Done_Qty, Remark, Qty, Outsourced_Cost, Inhouse_Cost, Cost_Save, TTL_Cost_Save FROM EST_DailyRawData ORDER BY StartDate DESC";
                
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new DailyRawData
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ww = reader["ww"].ToString(),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),                            
                            EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            CP = reader["CP"].ToString(),
                            Manager = reader["Manager"].ToString(),
                            Classification = reader["Classification"].ToString(),
                            Site = reader["Site"].ToString(),
                            Plant = reader["Plant"].ToString(),
                            Line = reader["Line"].ToString(),
                            Process = reader["Process"].ToString(),
                            Eq_Manufacturer = reader["Eq_Manufacturer"].ToString(),
                            Model_Name = reader["Model_Name"].ToString(),
                            MC = reader["MC"].ToString(),
                            Problem_Description = reader["Problem_Description"].ToString(),
                            Actions = reader["Actions"].ToString(),
                            Cause = reader["Cause"].ToString(),
                            Status = reader["Status"].ToString(),
                            TTL_Qty = reader["TTL_Qty"].ToString(),
                            Done_Qty = reader["Done_Qty"].ToString(),
                            Remark = reader["Remark"].ToString(),
                            Qty = reader["Qty"].ToString(),
                            Outsourced_Cost = reader["Outsourced_Cost"].ToString(),
                            Inhouse_Cost = reader["Inhouse_Cost"].ToString(),
                            Cost_Save = reader["Cost_Save"].ToString(),
                            TTL_Cost_Save = reader["TTL_Cost_Save"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public async Task InsertAsync(DailyRawData item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"INSERT INTO EST_DailyRawData
                                 (ww, StartDate, EndDate, CP, Manager, Classification, Site, Plant, Line, Process, 
                                  Eq_Manufacturer, Model_Name, MC, Problem_Description, Actions, Cause, Status,
                                  TTL_Qty, Done_Qty, Remark, Qty, Outsourced_Cost, Inhouse_Cost, Cost_Save, TTL_Cost_Save)
                                 VALUES 
                                  (@ww, @StartDate, @EndDate, @CP, @Manager, @Classification, @Site, @Plant, @Line, @Process,
                                   @Eq_Manufacturer, @Model_Name, @MC, @Problem_Description, @Actions, @Cause, @Status,
                                   @TTL_Qty, @Done_Qty, @Remark, @Qty, @Outsourced_Cost, @Inhouse_Cost, @Cost_Save, @TTL_Cost_Save)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // string 값이 null일 경우 빈 문자열로 대체
                    cmd.Parameters.AddWithValue("@ww", item.ww ?? "");
                    cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)item.EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CP", item.CP ?? "");
                    cmd.Parameters.AddWithValue("@Manager", item.Manager ?? "");
                    cmd.Parameters.AddWithValue("@Classification", item.Classification ?? "");
                    cmd.Parameters.AddWithValue("@Site", item.Site ?? "");
                    cmd.Parameters.AddWithValue("@Plant", item.Plant ?? "");
                    cmd.Parameters.AddWithValue("@Line", item.Line ?? "");
                    cmd.Parameters.AddWithValue("@Process", item.Process ?? "");
                    cmd.Parameters.AddWithValue("@Eq_Manufacturer", item.Eq_Manufacturer ?? "");
                    cmd.Parameters.AddWithValue("@Model_Name", item.Model_Name ?? "");
                    cmd.Parameters.AddWithValue("@MC", item.MC ?? "");
                    cmd.Parameters.AddWithValue("@Problem_Description", item.Problem_Description ?? "");
                    cmd.Parameters.AddWithValue("@Actions", item.Actions ?? "");
                    cmd.Parameters.AddWithValue("@Cause", item.Cause ?? "");
                    cmd.Parameters.AddWithValue("@Status", item.Status ?? "");
                    cmd.Parameters.AddWithValue("@TTL_Qty", item.TTL_Qty ?? "");
                    cmd.Parameters.AddWithValue("@Done_Qty", item.Done_Qty ?? "");
                    cmd.Parameters.AddWithValue("@Remark", item.Remark ?? "");
                    cmd.Parameters.AddWithValue("@Qty", item.Qty ?? "");
                    cmd.Parameters.AddWithValue("@Outsourced_Cost", item.Outsourced_Cost ?? "");
                    cmd.Parameters.AddWithValue("@Inhouse_Cost", item.Inhouse_Cost ?? "");
                    cmd.Parameters.AddWithValue("@Cost_Save", item.Cost_Save ?? "");
                    cmd.Parameters.AddWithValue("@TTL_Cost_Save", item.TTL_Cost_Save ?? "");                    

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<DailyRawData?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM EST_DailyRawData WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DailyRawData
                {
                    Id = id,
                    ww = reader["ww"].ToString(),
                    StartDate = reader["StartDate"] as DateTime?,
                    EndDate = reader["EndDate"] as DateTime?,
                    CP = reader["CP"].ToString(),
                    Manager = reader["Manager"].ToString(),
                    Classification = reader["Classification"].ToString(),
                    Site = reader["Site"].ToString(),
                    Plant = reader["Plant"].ToString(),
                    Line = reader["Line"].ToString(),
                    Process = reader["Process"].ToString(),
                    Eq_Manufacturer = reader["Eq_Manufacturer"].ToString(),
                    Model_Name = reader["Model_Name"].ToString(),
                    MC = reader["MC"].ToString(),
                    Problem_Description = reader["Problem_Description"].ToString(),
                    Actions = reader["Actions"].ToString(),
                    Cause = reader["Cause"].ToString(),
                    Status = reader["Status"].ToString(),
                    TTL_Qty = reader["TTL_Qty"].ToString(),
                    Done_Qty = reader["Done_Qty"].ToString(),
                    Remark = reader["Remark"].ToString(),
                    Qty = reader["Qty"].ToString(),
                    Outsourced_Cost = reader["Outsourced_Cost"].ToString(),
                    Inhouse_Cost = reader["Inhouse_Cost"].ToString(),
                    Cost_Save = reader["Cost_Save"].ToString(),
                    TTL_Cost_Save = reader["TTL_Cost_Save"].ToString()
                };
            }

            return null;
        }

        public async Task UpdateAsync(DailyRawData item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"UPDATE EST_DailyRawData SET
                                        ww = @ww, StartDate = @StartDate, EndDate = @EndDate,
                                        CP = @CP, Manager = @Manager, Classification = @Classification,
                                        Site = @Site, Plant = @Plant, Line = @Line, Process = @Process,
                                        Eq_Manufacturer = @Eq_Manufacturer, Model_Name = @Model_Name, MC = @MC,
                                        Problem_Description = @Problem_Description, Actions = @Actions, Cause = @Cause, Status = @Status,
                                        TTL_Qty = @TTL_Qty, Done_Qty = @Done_Qty, Remark = @Remark, Qty = @Qty,
                                        Outsourced_Cost = @Outsourced_Cost, Inhouse_Cost = @Inhouse_Cost, Cost_Save = @Cost_Save, TTL_Cost_Save = @TTL_Cost_Save
                                 WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", item.Id);
                    cmd.Parameters.AddWithValue("@ww", item.ww ?? "");
                    cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)item.EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CP", item.CP ?? "");
                    cmd.Parameters.AddWithValue("@Manager", item.Manager ?? "");
                    cmd.Parameters.AddWithValue("@Classification", item.Classification ?? "");
                    cmd.Parameters.AddWithValue("@Site", item.Site ?? "");
                    cmd.Parameters.AddWithValue("@Plant", item.Plant ?? "");
                    cmd.Parameters.AddWithValue("@Line", item.Line ?? "");
                    cmd.Parameters.AddWithValue("@Process", item.Process ?? "");
                    cmd.Parameters.AddWithValue("@Eq_Manufacturer", item.Eq_Manufacturer ?? "");
                    cmd.Parameters.AddWithValue("@Model_Name", item.Model_Name ?? "");
                    cmd.Parameters.AddWithValue("@MC", item.MC ?? "");
                    cmd.Parameters.AddWithValue("@Problem_Description", item.Problem_Description ?? "");
                    cmd.Parameters.AddWithValue("@Actions", item.Actions ?? "");
                    cmd.Parameters.AddWithValue("@Cause", item.Cause ?? "");
                    cmd.Parameters.AddWithValue("@Status", item.Status ?? "");
                    cmd.Parameters.AddWithValue("@TTL_Qty", item.TTL_Qty ?? "");
                    cmd.Parameters.AddWithValue("@Done_Qty", item.Done_Qty ?? "");
                    cmd.Parameters.AddWithValue("@Remark", item.Remark ?? "");
                    cmd.Parameters.AddWithValue("@Qty", item.Qty ?? "");
                    cmd.Parameters.AddWithValue("@Outsourced_Cost", item.Outsourced_Cost ?? "");
                    cmd.Parameters.AddWithValue("@Inhouse_Cost", item.Inhouse_Cost ?? "");
                    cmd.Parameters.AddWithValue("@Cost_Save", item.Cost_Save ?? "");
                    cmd.Parameters.AddWithValue("@TTL_Cost_Save", item.TTL_Cost_Save ?? "");
                    
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            
            var query = "DELETE FROM EST_DailyRawData WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            
            await cmd.ExecuteNonQueryAsync();
        }
    }
}