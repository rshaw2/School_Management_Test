using Microsoft.AspNetCore.Mvc;
using SchoolManagementTest.Models;
using SchoolManagementTest.Services;
using SchoolManagementTest.Entities;
using SchoolManagementTest.Filter;
using SchoolManagementTest.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace SchoolManagementTest.Controllers
{
    /// <summary>
    /// Controller responsible for managing studentdocuments related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting studentdocuments information.
    /// </remarks>
    [Route("api/studentdocuments")]
    [Authorize]
    public class StudentDocumentsController : ControllerBase
    {
        private readonly IStudentDocumentsService _studentDocumentsService;

        /// <summary>
        /// Initializes a new instance of the StudentDocumentsController class with the specified context.
        /// </summary>
        /// <param name="istudentdocumentsservice">The istudentdocumentsservice to be used by the controller.</param>
        public StudentDocumentsController(IStudentDocumentsService istudentdocumentsservice)
        {
            _studentDocumentsService = istudentdocumentsservice;
        }

        /// <summary>Adds a new studentdocuments</summary>
        /// <param name="model">The studentdocuments data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("StudentDocuments",Entitlements.Create)]
        public IActionResult Post([FromBody] StudentDocuments model)
        {
            var id = _studentDocumentsService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of studentdocumentss based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of studentdocumentss</returns>
        [HttpGet]
        [UserAuthorize("StudentDocuments",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] string filters, string searchTerm, int pageNumber = 1, int pageSize = 10, string sortField = null, string sortOrder = "asc")
        {
            List<FilterCriteria> filterCriteria = null;
            if (pageSize < 1)
            {
                return BadRequest("Page size invalid.");
            }

            if (pageNumber < 1)
            {
                return BadRequest("Page mumber invalid.");
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var result = _studentDocumentsService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <returns>The studentdocuments data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("StudentDocuments",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _studentDocumentsService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("StudentDocuments",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _studentDocumentsService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <param name="updatedEntity">The studentdocuments data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("StudentDocuments",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] StudentDocuments updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _studentDocumentsService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific studentdocuments by its primary key</summary>
        /// <param name="id">The primary key of the studentdocuments</param>
        /// <param name="updatedEntity">The studentdocuments data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("StudentDocuments",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<StudentDocuments> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _studentDocumentsService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}