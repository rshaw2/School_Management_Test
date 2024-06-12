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
    /// Controller responsible for managing chat related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting chat information.
    /// </remarks>
    [Route("api/chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        /// <summary>
        /// Initializes a new instance of the ChatController class with the specified context.
        /// </summary>
        /// <param name="ichatservice">The ichatservice to be used by the controller.</param>
        public ChatController(IChatService ichatservice)
        {
            _chatService = ichatservice;
        }

        /// <summary>Adds a new chat</summary>
        /// <param name="model">The chat data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("Chat",Entitlements.Create)]
        public IActionResult Post([FromBody] Chat model)
        {
            var id = _chatService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of chats based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of chats</returns>
        [HttpGet]
        [UserAuthorize("Chat",Entitlements.Read)]
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

            var result = _chatService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <returns>The chat data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("Chat",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _chatService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("Chat",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _chatService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <param name="updatedEntity">The chat data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("Chat",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] Chat updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _chatService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific chat by its primary key</summary>
        /// <param name="id">The primary key of the chat</param>
        /// <param name="updatedEntity">The chat data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("Chat",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<Chat> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _chatService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}