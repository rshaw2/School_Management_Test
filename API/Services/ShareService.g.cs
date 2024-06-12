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
    /// The shareService responsible for managing share related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting share information.
    /// </remarks>
    public interface IShareService
    {
        /// <summary>Retrieves a specific share by its primary key</summary>
        /// <param name="id">The primary key of the share</param>
        /// <returns>The share data</returns>
        Share GetById(Guid id);

        /// <summary>Retrieves a list of shares based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of shares</returns>
        List<Share> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new share</summary>
        /// <param name="model">The share data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Share model);

        /// <summary>Updates a specific share by its primary key</summary>
        /// <param name="id">The primary key of the share</param>
        /// <param name="updatedEntity">The share data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Share updatedEntity);

        /// <summary>Updates a specific share by its primary key</summary>
        /// <param name="id">The primary key of the share</param>
        /// <param name="updatedEntity">The share data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Share> updatedEntity);

        /// <summary>Deletes a specific share by its primary key</summary>
        /// <param name="id">The primary key of the share</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The shareService responsible for managing share related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting share information.
    /// </remarks>
    public class ShareService : IShareService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Share class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ShareService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific share by its primary key</summary>
        /// <param name="id">The primary key of the share</param>
        /// <returns>The share data</returns>
        public Share GetById(Guid id)
        {
            var entityData = _dbContext.Share.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of shares based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of shares</returns>/// <exception cref="Exception"></exception>
        public List<Share> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetShare(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new share</summary>
        /// <param name="model">The share data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Share model)
        {
            model.Id = CreateShare(model);
            return model.Id;
        }

        /// <summary>Updates a specific share by its primary key</summary>
        /// <param name="id">The primary key of the share</param>
        /// <param name="updatedEntity">The share data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Share updatedEntity)
        {
            UpdateShare(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific share by its primary key</summary>
        /// <param name="id">The primary key of the share</param>
        /// <param name="updatedEntity">The share data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Share> updatedEntity)
        {
            PatchShare(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific share by its primary key</summary>
        /// <param name="id">The primary key of the share</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteShare(id);
            return true;
        }
        #region
        private List<Share> GetShare(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Share.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Share>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Share), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Share, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateShare(Share model)
        {
            _dbContext.Share.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateShare(Guid id, Share updatedEntity)
        {
            _dbContext.Share.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteShare(Guid id)
        {
            var entityData = _dbContext.Share.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Share.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchShare(Guid id, JsonPatchDocument<Share> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Share.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Share.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}