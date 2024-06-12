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
    /// The resourcerequestService responsible for managing resourcerequest related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourcerequest information.
    /// </remarks>
    public interface IResourceRequestService
    {
        /// <summary>Retrieves a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <returns>The resourcerequest data</returns>
        ResourceRequest GetById(Guid id);

        /// <summary>Retrieves a list of resourcerequests based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcerequests</returns>
        List<ResourceRequest> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new resourcerequest</summary>
        /// <param name="model">The resourcerequest data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ResourceRequest model);

        /// <summary>Updates a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <param name="updatedEntity">The resourcerequest data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ResourceRequest updatedEntity);

        /// <summary>Updates a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <param name="updatedEntity">The resourcerequest data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ResourceRequest> updatedEntity);

        /// <summary>Deletes a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The resourcerequestService responsible for managing resourcerequest related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting resourcerequest information.
    /// </remarks>
    public class ResourceRequestService : IResourceRequestService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ResourceRequest class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ResourceRequestService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <returns>The resourcerequest data</returns>
        public ResourceRequest GetById(Guid id)
        {
            var entityData = _dbContext.ResourceRequest.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of resourcerequests based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of resourcerequests</returns>/// <exception cref="Exception"></exception>
        public List<ResourceRequest> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetResourceRequest(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new resourcerequest</summary>
        /// <param name="model">The resourcerequest data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ResourceRequest model)
        {
            model.Id = CreateResourceRequest(model);
            return model.Id;
        }

        /// <summary>Updates a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <param name="updatedEntity">The resourcerequest data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ResourceRequest updatedEntity)
        {
            UpdateResourceRequest(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <param name="updatedEntity">The resourcerequest data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ResourceRequest> updatedEntity)
        {
            PatchResourceRequest(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific resourcerequest by its primary key</summary>
        /// <param name="id">The primary key of the resourcerequest</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteResourceRequest(id);
            return true;
        }
        #region
        private List<ResourceRequest> GetResourceRequest(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ResourceRequest.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ResourceRequest>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ResourceRequest), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ResourceRequest, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateResourceRequest(ResourceRequest model)
        {
            _dbContext.ResourceRequest.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateResourceRequest(Guid id, ResourceRequest updatedEntity)
        {
            _dbContext.ResourceRequest.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteResourceRequest(Guid id)
        {
            var entityData = _dbContext.ResourceRequest.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ResourceRequest.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchResourceRequest(Guid id, JsonPatchDocument<ResourceRequest> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ResourceRequest.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ResourceRequest.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}