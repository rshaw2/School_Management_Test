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
    /// The timeoffapprovalService responsible for managing timeoffapproval related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timeoffapproval information.
    /// </remarks>
    public interface ITimeOffApprovalService
    {
        /// <summary>Retrieves a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <returns>The timeoffapproval data</returns>
        TimeOffApproval GetById(Guid id);

        /// <summary>Retrieves a list of timeoffapprovals based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeoffapprovals</returns>
        List<TimeOffApproval> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new timeoffapproval</summary>
        /// <param name="model">The timeoffapproval data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(TimeOffApproval model);

        /// <summary>Updates a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <param name="updatedEntity">The timeoffapproval data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, TimeOffApproval updatedEntity);

        /// <summary>Updates a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <param name="updatedEntity">The timeoffapproval data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<TimeOffApproval> updatedEntity);

        /// <summary>Deletes a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The timeoffapprovalService responsible for managing timeoffapproval related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting timeoffapproval information.
    /// </remarks>
    public class TimeOffApprovalService : ITimeOffApprovalService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the TimeOffApproval class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public TimeOffApprovalService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <returns>The timeoffapproval data</returns>
        public TimeOffApproval GetById(Guid id)
        {
            var entityData = _dbContext.TimeOffApproval.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of timeoffapprovals based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeoffapprovals</returns>/// <exception cref="Exception"></exception>
        public List<TimeOffApproval> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetTimeOffApproval(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new timeoffapproval</summary>
        /// <param name="model">The timeoffapproval data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(TimeOffApproval model)
        {
            model.Id = CreateTimeOffApproval(model);
            return model.Id;
        }

        /// <summary>Updates a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <param name="updatedEntity">The timeoffapproval data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, TimeOffApproval updatedEntity)
        {
            UpdateTimeOffApproval(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <param name="updatedEntity">The timeoffapproval data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<TimeOffApproval> updatedEntity)
        {
            PatchTimeOffApproval(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific timeoffapproval by its primary key</summary>
        /// <param name="id">The primary key of the timeoffapproval</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteTimeOffApproval(id);
            return true;
        }
        #region
        private List<TimeOffApproval> GetTimeOffApproval(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.TimeOffApproval.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<TimeOffApproval>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(TimeOffApproval), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<TimeOffApproval, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateTimeOffApproval(TimeOffApproval model)
        {
            _dbContext.TimeOffApproval.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateTimeOffApproval(Guid id, TimeOffApproval updatedEntity)
        {
            _dbContext.TimeOffApproval.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteTimeOffApproval(Guid id)
        {
            var entityData = _dbContext.TimeOffApproval.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.TimeOffApproval.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchTimeOffApproval(Guid id, JsonPatchDocument<TimeOffApproval> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.TimeOffApproval.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.TimeOffApproval.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}