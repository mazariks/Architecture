using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using WebApplication2.Controllers;
using Xunit;

namespace Tests.IntegrationTests;

public class EntryRepositoryTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private HttpClient _client = default!;
    private  CustomWebApplicationFactory<Program> _factory = default!;

    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions {AllowAutoRedirect = false });
    }

    [Test]
    public async Task Should_be_able_to_get_all_entries()
    {
        var sut = await GetContent<List<Entry>>("/Entry/Entries");
        
        sut.Count.Should().Be(1);
        sut.Single().Text.Should().Be("Some random text");
    }

    [Test]
    public async Task Should_be_able_to_get_exact_entry()
    {
        var entries = await GetContent<List<Entry>>("/Entry/Entries");
        
        var sut = await GetContent<Entry>($"/Entry/{entries.First().Id}");

        sut.Id.Should().Be(entries.First().Id);
        sut.Text.Should().Be("Some random text");
    }

    [Test]
    public async Task Should_be_able_to_load_entry_data()
    {
        var entries = await GetContent<List<Entry>>("/Entry/Entries");
        
        var sut = await GetContent<Entry>($"/Entry/WithData/{entries.First().Id}");

        sut.EntryDatas.Should().NotBeNull();
        sut.EntryDatas!.Count.Should().Be(2);
        sut.EntryDatas.All(data => data.EntryId == sut.Id).Should().BeTrue();
    }

    [Test]
    public async Task Should_be_able_to_get_only_entry_text()
    {
        var entries = await GetContent<List<Entry>>("/Entry/Entries");
        
        var sut = await GetContent<string>($"/Entry/EntryText/{entries.First().Id}");

        sut.Should().Be("Some random text");
    }

    [Test]
    public async Task Should_be_able_to_find_if_entry_has_same_text_as_description()
    {
        var entries = await GetContent<List<Entry>>("/Entry/Entries");

        var sut = await GetContent<bool>($"/Entry/WithSameTextAndDescriptionExists/{entries.First().Id}");
        
        sut.Should().BeTrue();
    }

    [Test]
    public async Task Should_be_able_to_add_entry()
    {
        var initialEntries = await GetContent<List<Entry>>("/Entry/Entries");

        await _client.PostAsJsonAsync("/Entry", 
            new AddEntryCommand("new Entry - Text", 100, "new Entry - description"));
        
        var entries = await GetContent<List<Entry>>("/Entry/Entries");

        entries.Count.Should().BeGreaterThan(initialEntries.Count);
    }
    
    [Test]
    public async Task Should_be_able_to_delete_entry()
    {
        var initialEntries = await GetContent<List<Entry>>("/Entry/Entries");

        await _client.DeleteAsync($"/Entry/{initialEntries.First().Id}", CancellationToken.None);
        
        var entries = await GetContent<List<Entry>>("/Entry/Entries");

        entries.Should().BeEmpty();
    }

    [Test]
    public async Task Should_be_able_to_edit_entry_text()
    {
        const string newText = "This is new text";
        var initialEntries = await GetContent<List<Entry>>("/Entry/Entries");

        await _client.PutAsJsonAsync($"/Entry/{initialEntries.First().Id}", newText);
        
        var entries = await GetContent<List<Entry>>("/Entry/Entries");

        entries.Single().Text.Should().Be(newText);
    }
    
    private async Task<T> GetContent<T>(string uri)
    {
        var response = await _client.GetAsync(uri);
        var content = await response.Content.ReadAsStringAsync();

        var deserializedData = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions 
            {PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        return deserializedData ?? throw new NullReferenceException("Data cannot be deserialised");
    }
}