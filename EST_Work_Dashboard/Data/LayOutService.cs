using EST_Work_Dashboard.Models;
using Microsoft.Data.SqlClient;

namespace EST_Work_Dashboard.Data
{
    public class LayOutService
    {
        private readonly string _connectionString;

        public LayOutService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<LayOutModel>> GetAllAsync()
        {
            var list = new List<LayOutModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query = "SELECT * FROM EST_LayOut ORDER BY StartDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new LayOutModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),                            
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            CP = reader["CP"].ToString(),
                            Manager = reader["Manager"].ToString(),
                            CR = reader["CR"].ToString(),
                            Project_Name = reader["Project_Name"].ToString(),
                            Plant = reader["Plant"].ToString(),
                            Line = reader["Line"].ToString(),
                            Process = reader["Process"].ToString(),
                            Eq_Manufacturer = reader["Eq_Manufacturer"].ToString(),
                            Model_Name = reader["Model_Name"].ToString(),
                            Classification = reader["Classification"].ToString(),
                            Remark = reader["Remark"].ToString()                            
                        });
                    }
                }
            }

            return list;
        }

        public async Task InsertAsync(LayOutModel item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"INSERT INTO EST_LayOut
                                 (StartDate, EndDate, CP, Manager, CR, Project_Name, Plant, Line, Process, 
                                  Eq_Manufacturer, Model_Name, Classification, Remark)
                                 VALUES 
                                  (@StartDate, @EndDate, @CP, @Manager, @CR, @Project_Name, @Plant, @Line, @Process,
                                   @Eq_Manufacturer, @Model_Name, @Classification, @Remark)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // string 값이 null일 경우 빈 문자열로 대체                    
                    cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)item.EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CP", item.CP ?? "");
                    cmd.Parameters.AddWithValue("@Manager", item.Manager ?? "");
                    cmd.Parameters.AddWithValue("@CR", item.CR ?? "");
                    cmd.Parameters.AddWithValue("@Project_Name", item.Project_Name ?? "");
                    cmd.Parameters.AddWithValue("@Plant", item.Plant ?? "");
                    cmd.Parameters.AddWithValue("@Line", item.Line ?? "");
                    cmd.Parameters.AddWithValue("@Process", item.Process ?? "");
                    cmd.Parameters.AddWithValue("@Eq_Manufacturer", item.Eq_Manufacturer ?? "");
                    cmd.Parameters.AddWithValue("@Model_Name", item.Model_Name ?? "");
                    cmd.Parameters.AddWithValue("@Classification", item.Classification ?? "");
                    cmd.Parameters.AddWithValue("@Remark", item.Remark ?? "");                    

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<LayOutModel?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM EST_LayOut WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new LayOutModel
                {
                    Id = id,                    
                    StartDate = reader["StartDate"] as DateTime?,
                    EndDate = reader["EndDate"] as DateTime?,
                    CP = reader["CP"].ToString(),
                    Manager = reader["Manager"].ToString(),
                    CR = reader["CR"].ToString(),
                    Project_Name = reader["Project_Name"].ToString(),
                    Plant = reader["Plant"].ToString(),
                    Line = reader["Line"].ToString(),
                    Process = reader["Process"].ToString(),
                    Eq_Manufacturer = reader["Eq_Manufacturer"].ToString(),
                    Model_Name = reader["Model_Name"].ToString(),
                    Classification = reader["Classification"].ToString(),                    
                    Remark = reader["Remark"].ToString(),                    
                };
            }

            return null;
        }

        public async Task UpdateAsync(LayOutModel item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"UPDATE EST_LayOut SET
                                        StartDate = @StartDate, EndDate = @EndDate,
                                        CP = @CP, Manager = @Manager, CR = @CR,
                                        Project_Name = @Project_Name, Plant = @Plant, Line = @Line, Process = @Process,
                                        Eq_Manufacturer = @Eq_Manufacturer, Model_Name = @Model_Name, Classification = @Classification,
                                        Remark = @Remark
                                 WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", item.Id);                    
                    cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)item.EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CP", item.CP ?? "");
                    cmd.Parameters.AddWithValue("@Manager", item.Manager ?? "");
                    cmd.Parameters.AddWithValue("@CR", item.CR ?? "");
                    cmd.Parameters.AddWithValue("@Project_Name", item.Project_Name ?? "");
                    cmd.Parameters.AddWithValue("@Plant", item.Plant ?? "");
                    cmd.Parameters.AddWithValue("@Line", item.Line ?? "");
                    cmd.Parameters.AddWithValue("@Process", item.Process ?? "");
                    cmd.Parameters.AddWithValue("@Eq_Manufacturer", item.Eq_Manufacturer ?? "");
                    cmd.Parameters.AddWithValue("@Model_Name", item.Model_Name ?? "");
                    cmd.Parameters.AddWithValue("@Classification", item.Classification ?? "");                    
                    cmd.Parameters.AddWithValue("@Remark", item.Remark ?? "");                    

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "DELETE FROM EST_LayOut WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
