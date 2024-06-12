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
    /// The facultyService responsible for managing faculty related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting faculty information.
    /// </remarks>
    public interface IFacultyService
    {
        /// <summary>Retrieves a specific faculty by its primary key</summary>
        /// <param name="id">The primary key of the faculty</param>
        /// <returns>The faculty data</returns>
        Faculty GetById(Guid id);

        /// <summary>Retrieves a list of facultys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of facultys</returns>
        List<Faculty> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new faculty</summary>
        /// <param name="model">The faculty data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Faculty model);

        /// <summary>Updates a specific faculty by its primary key</summary>
        /// <param name="id">The primary key of the faculty</param>
        /// <param name="updatedEntity">The faculty data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Faculty updatedEntity);

        /// <summary>Updates a specific faculty by its primary key</summary>
        /// <param name="id">The primary key of the faculty</param>
        /// <param name="updatedEntity">The faculty data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Faculty> updatedEntity);

        /// <summary>Deletes a specific faculty by its primary key</summary>
        /// <param name="id">The primary key of the faculty</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The facultyService responsible for managing faculty related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting faculty information.
    /// </remarks>
    public class FacultyService : IFacultyService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Faculty class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public FacultyService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific faculty by its primary key</summary>
        /// <param name="id">The primary key of the faculty</param>
        /// <returns>The faculty data</returns>
        public Faculty GetById(Guid id)
        {
            var entityData = _dbContext.Faculty.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of facultys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of facultys</returns>/// <exception cref="Exception"></exception>
        public List<Faculty> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetFaculty(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new faculty</summary>
        /// <param name="model">The faculty data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Faculty model)
        {
            model.Id = CreateFaculty(model);
            return model.Id;
        }

        /// <summary>Updates a specific faculty by its primary key</summary>
        /// <param name="id">The primary key of the faculty</param>
        /// <param name="updatedEntity">The faculty data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Faculty updatedEntity)
        {
            UpdateFaculty(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific faculty by its primary key</summary>
        /// <param name="id">The primary key of the faculty</param>
        /// <param name="updatedEntity">The faculty data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Faculty> updatedEntity)
        {
            PatchFaculty(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific faculty by its primary key</summary>
        /// <param name="id">The primary key of the faculty</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteFaculty(id);
            return true;
        }
        #region
        private List<Faculty> GetFaculty(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Faculty.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Faculty>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Faculty), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Faculty, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateFaculty(Faculty model)
        {
            _dbContext.Faculty.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateFaculty(Guid id, Faculty updatedEntity)
        {
            _dbContext.Faculty.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteFaculty(Guid id)
        {
            var entityData = _dbContext.Faculty.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Faculty.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchFaculty(Guid id, JsonPatchDocument<Faculty> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Faculty.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Faculty.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}