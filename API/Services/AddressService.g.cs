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
    /// The addressService responsible for managing address related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting address information.
    /// </remarks>
    public interface IAddressService
    {
        /// <summary>Retrieves a specific address by its primary key</summary>
        /// <param name="id">The primary key of the address</param>
        /// <returns>The address data</returns>
        Address GetById(Guid id);

        /// <summary>Retrieves a list of addresss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of addresss</returns>
        List<Address> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new address</summary>
        /// <param name="model">The address data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Address model);

        /// <summary>Updates a specific address by its primary key</summary>
        /// <param name="id">The primary key of the address</param>
        /// <param name="updatedEntity">The address data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Address updatedEntity);

        /// <summary>Updates a specific address by its primary key</summary>
        /// <param name="id">The primary key of the address</param>
        /// <param name="updatedEntity">The address data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Address> updatedEntity);

        /// <summary>Deletes a specific address by its primary key</summary>
        /// <param name="id">The primary key of the address</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The addressService responsible for managing address related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting address information.
    /// </remarks>
    public class AddressService : IAddressService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Address class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AddressService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific address by its primary key</summary>
        /// <param name="id">The primary key of the address</param>
        /// <returns>The address data</returns>
        public Address GetById(Guid id)
        {
            var entityData = _dbContext.Address.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of addresss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of addresss</returns>/// <exception cref="Exception"></exception>
        public List<Address> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAddress(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new address</summary>
        /// <param name="model">The address data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Address model)
        {
            model.Id = CreateAddress(model);
            return model.Id;
        }

        /// <summary>Updates a specific address by its primary key</summary>
        /// <param name="id">The primary key of the address</param>
        /// <param name="updatedEntity">The address data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Address updatedEntity)
        {
            UpdateAddress(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific address by its primary key</summary>
        /// <param name="id">The primary key of the address</param>
        /// <param name="updatedEntity">The address data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Address> updatedEntity)
        {
            PatchAddress(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific address by its primary key</summary>
        /// <param name="id">The primary key of the address</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAddress(id);
            return true;
        }
        #region
        private List<Address> GetAddress(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Address.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Address>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Address), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Address, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateAddress(Address model)
        {
            _dbContext.Address.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAddress(Guid id, Address updatedEntity)
        {
            _dbContext.Address.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAddress(Guid id)
        {
            var entityData = _dbContext.Address.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Address.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAddress(Guid id, JsonPatchDocument<Address> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Address.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Address.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}