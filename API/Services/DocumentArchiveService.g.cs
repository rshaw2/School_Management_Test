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
    /// The documentarchiveService responsible for managing documentarchive related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documentarchive information.
    /// </remarks>
    public interface IDocumentArchiveService
    {
        /// <summary>Retrieves a specific documentarchive by its primary key</summary>
        /// <param name="id">The primary key of the documentarchive</param>
        /// <returns>The documentarchive data</returns>
        DocumentArchive GetById(Guid id);

        /// <summary>Retrieves a list of documentarchives based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documentarchives</returns>
        List<DocumentArchive> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new documentarchive</summary>
        /// <param name="model">The documentarchive data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(DocumentArchive model);

        /// <summary>Updates a specific documentarchive by its primary key</summary>
        /// <param name="id">The primary key of the documentarchive</param>
        /// <param name="updatedEntity">The documentarchive data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, DocumentArchive updatedEntity);

        /// <summary>Updates a specific documentarchive by its primary key</summary>
        /// <param name="id">The primary key of the documentarchive</param>
        /// <param name="updatedEntity">The documentarchive data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<DocumentArchive> updatedEntity);

        /// <summary>Deletes a specific documentarchive by its primary key</summary>
        /// <param name="id">The primary key of the documentarchive</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The documentarchiveService responsible for managing documentarchive related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting documentarchive information.
    /// </remarks>
    public class DocumentArchiveService : IDocumentArchiveService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the DocumentArchive class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DocumentArchiveService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific documentarchive by its primary key</summary>
        /// <param name="id">The primary key of the documentarchive</param>
        /// <returns>The documentarchive data</returns>
        public DocumentArchive GetById(Guid id)
        {
            var entityData = _dbContext.DocumentArchive.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of documentarchives based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of documentarchives</returns>/// <exception cref="Exception"></exception>
        public List<DocumentArchive> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDocumentArchive(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new documentarchive</summary>
        /// <param name="model">The documentarchive data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(DocumentArchive model)
        {
            model.Id = CreateDocumentArchive(model);
            return model.Id;
        }

        /// <summary>Updates a specific documentarchive by its primary key</summary>
        /// <param name="id">The primary key of the documentarchive</param>
        /// <param name="updatedEntity">The documentarchive data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, DocumentArchive updatedEntity)
        {
            UpdateDocumentArchive(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific documentarchive by its primary key</summary>
        /// <param name="id">The primary key of the documentarchive</param>
        /// <param name="updatedEntity">The documentarchive data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<DocumentArchive> updatedEntity)
        {
            PatchDocumentArchive(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific documentarchive by its primary key</summary>
        /// <param name="id">The primary key of the documentarchive</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDocumentArchive(id);
            return true;
        }
        #region
        private List<DocumentArchive> GetDocumentArchive(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.DocumentArchive.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<DocumentArchive>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(DocumentArchive), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<DocumentArchive, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDocumentArchive(DocumentArchive model)
        {
            _dbContext.DocumentArchive.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDocumentArchive(Guid id, DocumentArchive updatedEntity)
        {
            _dbContext.DocumentArchive.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDocumentArchive(Guid id)
        {
            var entityData = _dbContext.DocumentArchive.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.DocumentArchive.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDocumentArchive(Guid id, JsonPatchDocument<DocumentArchive> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.DocumentArchive.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.DocumentArchive.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}