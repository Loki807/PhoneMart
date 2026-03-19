using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PhoneMart.API.Controllers;
using PhoneMart.Application.Features.Public.DTOs;
using PhoneMart.Application.Features.Public.Requests.Queries;

namespace PhoneMart.Tests;

public class PublicControllerTests
{
    [Fact]
    public async Task SearchProducts_ReturnsOkResult_WithProducts()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var dbMock = new Mock<PhoneMart.Application.Contracts.Persistence.IAppDbContext>();
        
        var controller = new PublicController(mediatorMock.Object, dbMock.Object);
        var expectedProducts = new List<PublicProductDto>
        {
            new PublicProductDto { Id = Guid.NewGuid(), Title = "iPhone 15", Price = 300000 }
        };

        mediatorMock
            .Setup(m => m.Send(It.IsAny<SearchProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProducts);

        // Act
        var result = await controller.SearchProducts("iPhone", null, null, null, 1, 20);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsType<List<PublicProductDto>>(okResult.Value);
        Assert.Single(returnedProducts);
        Assert.Equal("iPhone 15", returnedProducts[0].Title);
    }
}
