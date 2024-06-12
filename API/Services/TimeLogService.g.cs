using SchoolManagementTest.Models;
using SchoolManagementTest.Data;
using SchoolManagementTest.Filter;
using SchoolManagementTest.Entities;
using SchoolManagementTest.Logger;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace SchoolManagementTest.Services
{
    /// <summary>
    /// The timelogService responsible for managing timelog related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timelog information.
    /// </remarks>
    public interface ITimeLogService
    {
        /// <summary>Retrieves a specific timelog by its primary key</summary>
        /// <param name="id">The primary key of the timelog</param>
        /// <returns>The timelog data</returns>
        TimeLog GetById(Guid id);

        /// <summary>Retrieves a list of timelogs based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timelogs</returns>
        List<TimeLog> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new timelog</summary>
        /// <param name="model">The timelog data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(TimeLog model);

        /// <summary>Updates a specific timelog by its primary key</summary>
        /// <param name="id">The primary key of the timelog</param>
        /// <param name="updatedEntity">The timelog data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, TimeLog updatedEntity);

        /// <summary>Updates a specific timelog by its primary key</summary>
        /// <param name="id">The primary key of the timelog</param>
        /// <param name="updatedEntity">The timelog data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<TimeLog> updatedEntity);

        /// <summary>Deletes a specific timelog by its primary key</summary>
        /// <param name="id">The primary key of the timelog</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The timelogService responsible for managing timelog related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timelog information.
    /// </remarks>
    public class TimeLogService : ITimeLogService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the TimeLog class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TimeLogService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific timelog by its primary key</summary>
        /// <param name="id">The primary key of the timelog</param>
        /// <returns>The timelog data</returns>
        public TimeLog GetById(Guid id)
        {
            var entityData = _dbContext.TimeLog.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of timelogs based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timelogs</returns>/// <exception cref="Exception"></exception>
        public List<TimeLog> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTimeLog(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new timelog</summary>
        /// <param name="model">The timelog data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(TimeLog model)
        {
            model.Id = CreateTimeLog(model);
            return model.Id;
        }

        /// <summary>Updates a specific timelog by its primary key</summary>
        /// <param name="id">The primary key of the timelog</param>
        /// <param name="updatedEntity">The timelog data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, TimeLog updatedEntity)
        {
            UpdateTimeLog(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific timelog by its primary key</summary>
        /// <param name="id">The primary key of the timelog</param>
        /// <param name="updatedEntity">The timelog data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<TimeLog> updatedEntity)
        {
            PatchTimeLog(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific timelog by its primary key</summary>
        /// <param name="id">The primary key of the timelog</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTimeLog(id);
            return true;
        }
        #region
        private List<TimeLog> GetTimeLog(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.TimeLog.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<TimeLog>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(TimeLog), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<TimeLog, object>>(Expression.Convert(property, typeof(object)), parameter);
                if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(lambda);
                }
                else if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderByDescending(lambda);
                }
                else
                {
                    throw new ApplicationException("Invalid sort order. Use 'asc' or 'desc'");
                }
            }

            var paginatedResult = result.Skip(skip).Take(pageSize).ToList();
            return paginatedResult;
        }

        private Guid CreateTimeLog(TimeLog model)
        {
            _dbContext.TimeLog.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTimeLog(Guid id, TimeLog updatedEntity)
        {
            _dbContext.TimeLog.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTimeLog(Guid id)
        {
            var entityData = _dbContext.TimeLog.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.TimeLog.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTimeLog(Guid id, JsonPatchDocument<TimeLog> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.TimeLog.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.TimeLog.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}