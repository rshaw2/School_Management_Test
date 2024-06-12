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
    /// The resourcestatusService responsible for managing resourcestatus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourcestatus information.
    /// </remarks>
    public interface IResourceStatusService
    {
        /// <summary>Retrieves a specific resourcestatus by its primary key</summary>
        /// <param name="id">The primary key of the resourcestatus</param>
        /// <returns>The resourcestatus data</returns>
        ResourceStatus GetById(Guid id);

        /// <summary>Retrieves a list of resourcestatuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcestatuss</returns>
        List<ResourceStatus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new resourcestatus</summary>
        /// <param name="model">The resourcestatus data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ResourceStatus model);

        /// <summary>Updates a specific resourcestatus by its primary key</summary>
        /// <param name="id">The primary key of the resourcestatus</param>
        /// <param name="updatedEntity">The resourcestatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ResourceStatus updatedEntity);

        /// <summary>Updates a specific resourcestatus by its primary key</summary>
        /// <param name="id">The primary key of the resourcestatus</param>
        /// <param name="updatedEntity">The resourcestatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ResourceStatus> updatedEntity);

        /// <summary>Deletes a specific resourcestatus by its primary key</summary>
        /// <param name="id">The primary key of the resourcestatus</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The resourcestatusService responsible for managing resourcestatus related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourcestatus information.
    /// </remarks>
    public class ResourceStatusService : IResourceStatusService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ResourceStatus class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ResourceStatusService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific resourcestatus by its primary key</summary>
        /// <param name="id">The primary key of the resourcestatus</param>
        /// <returns>The resourcestatus data</returns>
        public ResourceStatus GetById(Guid id)
        {
            var entityData = _dbContext.ResourceStatus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of resourcestatuss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcestatuss</returns>/// <exception cref="Exception"></exception>
        public List<ResourceStatus> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetResourceStatus(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new resourcestatus</summary>
        /// <param name="model">The resourcestatus data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ResourceStatus model)
        {
            model.Id = CreateResourceStatus(model);
            return model.Id;
        }

        /// <summary>Updates a specific resourcestatus by its primary key</summary>
        /// <param name="id">The primary key of the resourcestatus</param>
        /// <param name="updatedEntity">The resourcestatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ResourceStatus updatedEntity)
        {
            UpdateResourceStatus(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific resourcestatus by its primary key</summary>
        /// <param name="id">The primary key of the resourcestatus</param>
        /// <param name="updatedEntity">The resourcestatus data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ResourceStatus> updatedEntity)
        {
            PatchResourceStatus(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific resourcestatus by its primary key</summary>
        /// <param name="id">The primary key of the resourcestatus</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteResourceStatus(id);
            return true;
        }
        #region
        private List<ResourceStatus> GetResourceStatus(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ResourceStatus.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ResourceStatus>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ResourceStatus), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ResourceStatus, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateResourceStatus(ResourceStatus model)
        {
            _dbContext.ResourceStatus.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateResourceStatus(Guid id, ResourceStatus updatedEntity)
        {
            _dbContext.ResourceStatus.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteResourceStatus(Guid id)
        {
            var entityData = _dbContext.ResourceStatus.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ResourceStatus.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchResourceStatus(Guid id, JsonPatchDocument<ResourceStatus> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ResourceStatus.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ResourceStatus.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}