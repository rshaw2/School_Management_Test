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
    /// The currencyService responsible for managing currency related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting currency information.
    /// </remarks>
    public interface ICurrencyService
    {
        /// <summary>Retrieves a specific currency by its primary key</summary>
        /// <param name="id">The primary key of the currency</param>
        /// <returns>The currency data</returns>
        Currency GetById(Guid id);

        /// <summary>Retrieves a list of currencys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of currencys</returns>
        List<Currency> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new currency</summary>
        /// <param name="model">The currency data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Currency model);

        /// <summary>Updates a specific currency by its primary key</summary>
        /// <param name="id">The primary key of the currency</param>
        /// <param name="updatedEntity">The currency data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Currency updatedEntity);

        /// <summary>Updates a specific currency by its primary key</summary>
        /// <param name="id">The primary key of the currency</param>
        /// <param name="updatedEntity">The currency data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Currency> updatedEntity);

        /// <summary>Deletes a specific currency by its primary key</summary>
        /// <param name="id">The primary key of the currency</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The currencyService responsible for managing currency related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting currency information.
    /// </remarks>
    public class CurrencyService : ICurrencyService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Currency class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public CurrencyService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific currency by its primary key</summary>
        /// <param name="id">The primary key of the currency</param>
        /// <returns>The currency data</returns>
        public Currency GetById(Guid id)
        {
            var entityData = _dbContext.Currency.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of currencys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of currencys</returns>/// <exception cref="Exception"></exception>
        public List<Currency> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetCurrency(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new currency</summary>
        /// <param name="model">The currency data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Currency model)
        {
            model.Id = CreateCurrency(model);
            return model.Id;
        }

        /// <summary>Updates a specific currency by its primary key</summary>
        /// <param name="id">The primary key of the currency</param>
        /// <param name="updatedEntity">The currency data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Currency updatedEntity)
        {
            UpdateCurrency(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific currency by its primary key</summary>
        /// <param name="id">The primary key of the currency</param>
        /// <param name="updatedEntity">The currency data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Currency> updatedEntity)
        {
            PatchCurrency(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific currency by its primary key</summary>
        /// <param name="id">The primary key of the currency</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteCurrency(id);
            return true;
        }
        #region
        private List<Currency> GetCurrency(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Currency.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Currency>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Currency), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Currency, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateCurrency(Currency model)
        {
            _dbContext.Currency.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateCurrency(Guid id, Currency updatedEntity)
        {
            _dbContext.Currency.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteCurrency(Guid id)
        {
            var entityData = _dbContext.Currency.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Currency.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchCurrency(Guid id, JsonPatchDocument<Currency> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Currency.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Currency.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}