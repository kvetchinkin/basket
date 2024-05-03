using BasketApp.Api.Adapters.Http.Contract.src.Api.Controllers;
using BasketApp.Api.Adapters.Http.Contract.src.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BasketApp.Api.Adapters.Http;

public class BasketController : DefaultApiController
{
    private readonly IMediator _mediator;

    public BasketController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<IActionResult> AddAddress(Guid basketId, Address address)
    {
        var addAddressCommand = new Core.Application.UseCases.Commands.AddAddress.Command(basketId, address.Country,
            address.City, address.Street, address.House, address.Apartment);

        var response = await _mediator.Send(addAddressCommand);
        if (response) return Ok();
        return Conflict();
    }

    public override async Task<IActionResult> AddDeliveryPeriod(Guid basketId, string body)
    {
        var result = Enum.TryParse(body, out Core.Application.UseCases.Commands.AddDeliveryPeriod.TimeSlot timeSlot);
        if (!result)
        {
            return Conflict();
        }

        var addDeliveryPeriodCommand =
            new Core.Application.UseCases.Commands.AddDeliveryPeriod.Command(basketId, timeSlot);

        var response = await _mediator.Send(addDeliveryPeriodCommand);
        if (response) return Ok();
        return Conflict();
    }

    public override async Task<IActionResult> ChangeItems(Guid basketId, Item item)
    {
        var changeItemsCommand =
            new Core.Application.UseCases.Commands.ChangeItems.Command(basketId, item.GoodId, item.Quantity);

        var response = await _mediator.Send(changeItemsCommand);
        if (response) return Ok();
        return Conflict();
    }

    public override async Task<IActionResult> Checkout(Guid basketId)
    {
        var changeItemsCommand = new Core.Application.UseCases.Commands.Checkout.Command(basketId);

        var response = await _mediator.Send(changeItemsCommand);
        if (response) return Ok();
        return Conflict();
    }

    public override async Task<IActionResult> GetBasketItems(Guid basketId)
    {
        var getBasketQuery = new Core.Application.UseCases.Queries.GetBasket.Query(basketId);

        var response = await _mediator.Send(getBasketQuery);
        return Ok(response);
    }
}