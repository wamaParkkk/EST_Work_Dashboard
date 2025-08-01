using EST_Work_Dashboard.Models;
using Microsoft.Data.SqlClient;

namespace EST_Work_Dashboard.Data
{
    public class TrainingService
    {
        private readonly string _connectionString;

        public TrainingService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<TrainingModel>> GetAllAsync()
        {
            var list = new List<TrainingModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query = "SELECT * FROM EST_Training ORDER BY StartDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new TrainingModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            CP = reader["CP"].ToString(),
                            Manager = reader["Manager"].ToString(),
                            Site = reader["Site"].ToString(),                            
                            Plant = reader["Plant"].ToString(),                            
                            Process = reader["Process"].ToString(),
                            Eq_Manufacturer = reader["Eq_Manufacturer"].ToString(),
                            Model_Name = reader["Model_Name"].ToString(),
                            Training_Content = reader["Training_Content"].ToString(),
                            Trainer = reader["Trainer"].ToString(),
                            Trainee = reader["Trainee"].ToString(),
                            Remark = reader["Remark"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public async Task InsertAsync(TrainingModel item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"INSERT INTO EST_Training
                                 (StartDate, EndDate, CP, Manager, Site, Plant, Process, 
                                  Eq_Manufacturer, Model_Name, Training_Content, Trainer, Trainee, Remark)
                                 VALUES 
                                  (@StartDate, @EndDate, @CP, @Manager, @Site, @Plant, @Process,
                                   @Eq_Manufacturer, @Model_Name, @Training_Content, @Trainer, @Trainee, @Remark)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // string 값이 null일 경우 빈 문자열로 대체                    
                    cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)item.EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CP", item.CP ?? "");
                    cmd.Parameters.AddWithValue("@Manager", item.Manager ?? "");
                    cmd.Parameters.AddWithValue("@Site", item.Site ?? "");                    
                    cmd.Parameters.AddWithValue("@Plant", item.Plant ?? "");                    
                    cmd.Parameters.AddWithValue("@Process", item.Process ?? "");
                    cmd.Parameters.AddWithValue("@Eq_Manufacturer", item.Eq_Manufacturer ?? "");
                    cmd.Parameters.AddWithValue("@Model_Name", item.Model_Name ?? "");
                    cmd.Parameters.AddWithValue("@Training_Content", item.Training_Content ?? "");
                    cmd.Parameters.AddWithValue("@Trainer", item.Trainer ?? "");
                    cmd.Parameters.AddWithValue("@Trainee", item.Trainee ?? "");
                    cmd.Parameters.AddWithValue("@Remark", item.Remark ?? "");

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<TrainingModel?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM EST_Training WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new TrainingModel
                {
                    Id = id,
                    StartDate = reader["StartDate"] as DateTime?,
                    EndDate = reader["EndDate"] as DateTime?,
                    CP = reader["CP"].ToString(),
                    Manager = reader["Manager"].ToString(),
                    Site = reader["Site"].ToString(),                    
                    Plant = reader["Plant"].ToString(),                    
                    Process = reader["Process"].ToString(),
                    Eq_Manufacturer = reader["Eq_Manufacturer"].ToString(),
                    Model_Name = reader["Model_Name"].ToString(),
                    Training_Content = reader["Training_Content"].ToString(),
                    Trainer = reader["Trainer"].ToString(),
                    Trainee = reader["Trainee"].ToString(),
                    Remark = reader["Remark"].ToString(),
                };
            }

            return null;
        }

        public async Task UpdateAsync(TrainingModel item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"UPDATE EST_Training SET
                                        StartDate = @StartDate, EndDate = @EndDate,
                                        CP = @CP, Manager = @Manager,
                                        Site = @Site, Plant = @Plant, Process = @Process,
                                        Eq_Manufacturer = @Eq_Manufacturer, Model_Name = @Model_Name,
                                        Training_Content = @Training_Content, Trainer = @Trainer, Trainee = @Trainee,
                                        Remark = @Remark
                                 WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", item.Id);
                    cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)item.EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CP", item.CP ?? "");
                    cmd.Parameters.AddWithValue("@Manager", item.Manager ?? "");
                    cmd.Parameters.AddWithValue("@Site", item.Site ?? "");                    
                    cmd.Parameters.AddWithValue("@Plant", item.Plant ?? "");                    
                    cmd.Parameters.AddWithValue("@Process", item.Process ?? "");
                    cmd.Parameters.AddWithValue("@Eq_Manufacturer", item.Eq_Manufacturer ?? "");
                    cmd.Parameters.AddWithValue("@Model_Name", item.Model_Name ?? "");
                    cmd.Parameters.AddWithValue("@Training_Content", item.Training_Content ?? "");
                    cmd.Parameters.AddWithValue("@Trainer", item.Trainer ?? "");
                    cmd.Parameters.AddWithValue("@Trainee", item.Trainee ?? "");
                    cmd.Parameters.AddWithValue("@Remark", item.Remark ?? "");

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "DELETE FROM EST_Training WHERE Id = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
