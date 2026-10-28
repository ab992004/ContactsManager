using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CRUDTests
{
  public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
  {
    private readonly HttpClient _client;

    public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
    {
      _client = factory.CreateClient();
    }


    #region Index
    [Fact]
    public async Task Index_ToReturnView()
    {
      //Arrange



      //Act
      HttpResponseMessage response = await _client.GetAsync("/Persons/Index");


      //Assert
      response.IsSuccessStatusCode.Should().BeTrue();

      string responseContent = await response.Content.ReadAsStringAsync();
      HtmlDocument html = new HtmlDocument();
      html.LoadHtml(responseContent);

      var document = html.DocumentNode;
      document.QuerySelectorAll("table.persons").Should().NotBeNull();
    }
    #endregion
  }
}
