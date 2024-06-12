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
    /// The parentguardianService responsible for managing parentguardian related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting parentguardian information.
    /// </remarks>
    public interface IParentGuardianService
    {
        /// <summary>Retrieves a specific parentguardian by its primary key</summary>
        /// <param name="id">The primary key of the parentguardian</param>
        /// <returns>The parentguardian data</returns>
        ParentGuardian GetById(Guid id);

        /// <summary>Retrieves a list of parentguardians based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of parentguardians</returns>
        List<ParentGuardian> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new parentguardian</summary>
        /// <param name="model">The parentguardian data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ParentGuardian model);

        /// <summary>Updates a specific parentguardian by its primary key</summary>
        /// <param name="id">The primary key of the parentguardian</param>
        /// <param name="updatedEntity">The parentguardian data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ParentGuardian updatedEntity);

        /// <summary>Updates a specific parentguardian by its primary key</summary>
        /// <param name="id">The primary key of the parentguardian</param>
        /// <param name="updatedEntity">The parentguardian data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ParentGuardian> updatedEntity);

        /// <summary>Deletes a specific parentguardian by its primary key</summary>
        /// <param name="id">The primary key of the parentguardian</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The parentguardianService responsible for managing parentguardian related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting parentguardian information.
    /// </remarks>
    public class ParentGuardianService : IParentGuardianService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ParentGuardian class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ParentGuardianService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific parentguardian by its primary key</summary>
        /// <param name="id">The primary key of the parentguardian</param>
        /// <returns>The parentguardian data</returns>
        public ParentGuardian GetById(Guid id)
        {
            var entityData = _dbContext.ParentGuardian.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of parentguardians based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of parentguardians</returns>/// <exception cref="Exception"></exception>
        public List<ParentGuardian> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetParentGuardian(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new parentguardian</summary>
        /// <param name="model">The parentguardian data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ParentGuardian model)
        {
            model.Id = CreateParentGuardian(model);
            return model.Id;
        }

        /// <summary>Updates a specific parentguardian by its primary key</summary>
        /// <param name="id">The primary key of the parentguardian</param>
        /// <param name="updatedEntity">The parentguardian data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ParentGuardian updatedEntity)
        {
            UpdateParentGuardian(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific parentguardian by its primary key</summary>
        /// <param name="id">The primary key of the parentguardian</param>
        /// <param name="updatedEntity">The parentguardian data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ParentGuardian> updatedEntity)
        {
            PatchParentGuardian(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific parentguardian by its primary key</summary>
        /// <param name="id">The primary key of the parentguardian</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteParentGuardian(id);
            return true;
        }
        #region
        private List<ParentGuardian> GetParentGuardian(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ParentGuardian.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ParentGuardian>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ParentGuardian), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ParentGuardian, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateParentGuardian(ParentGuardian model)
        {
            _dbContext.ParentGuardian.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateParentGuardian(Guid id, ParentGuardian updatedEntity)
        {
            _dbContext.ParentGuardian.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteParentGuardian(Guid id)
        {
            var entityData = _dbContext.ParentGuardian.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ParentGuardian.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchParentGuardian(Guid id, JsonPatchDocument<ParentGuardian> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ParentGuardian.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ParentGuardian.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}