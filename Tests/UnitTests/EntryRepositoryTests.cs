using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DALContracts;
using Domain;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using WebApplication2.Controllers;

namespace Tests.UnitTests;

public class EntryRepositoryTests
{
    [Test]
    public async Task Should_get_right_entry()
    {
        var entryId = Guid.NewGuid();
        
        var mockedEntry = Mock.Of<Entry>(x => 
            x.Text == "123" && 
            x.Id == entryId);
        
        var repositoryMock = new Mock<IEntryRepository>();
        repositoryMock.Setup(m => 
                m.GetByIdAsync(entryId, CancellationToken.None))
            .ReturnsAsync(mockedEntry)
            .Verifiable();

        var controller = new EntryController(repositoryMock.Object);

        var result = await controller.GetEntry(entryId);

        result.Id.Should().Be(entryId);
        repositoryMock.Verify();
    }
    
    [Test]
    public void Should_throw_when_entry_not_found()
    {
        var entryId = Guid.NewGuid();
        
        var repositoryMock = new Mock<IEntryRepository>();
        repositoryMock.Setup(m => 
                m.GetByIdAsync(entryId, CancellationToken.None))
            .ReturnsAsync(value: null)
            .Verifiable();

        var controller = new EntryController(repositoryMock.Object);

        Assert.ThrowsAsync<Exception>(() => controller.GetEntry(entryId))!.Message.Should().Be("Entry was not found");
        
        repositoryMock.Verify();
    }

    [Test]
    public async Task Should_get_entry_with_data()
    {
        var entryId = Guid.NewGuid();
        var entryDataId = Guid.NewGuid();

        var mockedEntryData = Mock.Of<EntryData>(x => 
            x.Amount == 2 && x.EntryId == entryId && x.Id == entryDataId);
        
        var mockedEntry = Mock.Of<Entry>(x => 
            x.Text == "123" && 
            x.Id == entryId && 
            x.EntryDatas == new List<EntryData> {mockedEntryData});
        
        var repositoryMock = new Mock<IEntryRepository>();
        repositoryMock.Setup(m => 
                m.GetWithDataAsync(entryId, CancellationToken.None))
            .ReturnsAsync(mockedEntry).Verifiable();
        
        var controller = new EntryController(repositoryMock.Object);

        var result = await controller.GetEntryWithData(entryId);

        result.EntryDatas.Should().NotBeEmpty();
        result.EntryDatas!.Single().Id.Should().Be(entryDataId);
        
        repositoryMock.Verify();
    }

    [Test]
    public async Task Should_get_entry_text()
    {
        var entryId = Guid.NewGuid();
        
        var mockedEntry = Mock.Of<Entry>(x => 
            x.Text == "123" && 
            x.Id == entryId);
        
        var repositoryMock = new Mock<IEntryRepository>();
        repositoryMock.Setup(m => 
                m.GetEntryTextAsync(entryId, CancellationToken.None))
            .ReturnsAsync(mockedEntry.Text)
            .Verifiable();

        var controller = new EntryController(repositoryMock.Object);

        var result = await controller.GetEntryText(entryId);

        result.Should().Be(mockedEntry.Text);
        repositoryMock.Verify();
    }

    [Test]
    public async Task Should_be_true_when_entry_text_is_same_as_any_description()
    {
        var entryId = Guid.NewGuid();
        
        var repositoryMock = new Mock<IEntryRepository>();
        repositoryMock.Setup(m => 
                m.AnyEntryHasSameTextAndDescriptionAsync(entryId, CancellationToken.None))
            .ReturnsAsync(true).Verifiable();
        
        var controller = new EntryController(repositoryMock.Object);

        var result = await controller.GetAnyEntryHasTheSameTextAndDescription(entryId);

        result.Should().BeTrue();
        repositoryMock.Verify();
    }
    
    [Test]
    public async Task Should_invoke_removal_of_entry()
    {
        var entryId = Guid.NewGuid();
        
        var mockedEntry = Mock.Of<Entry>(x => 
            x.Text == "123" && 
            x.Id == entryId);
        
        var repositoryMock = new Mock<IEntryRepository>();
        var controller = new EntryController(repositoryMock.Object);
        
        repositoryMock.Setup(m => m.SaveEntryAsync(mockedEntry, CancellationToken.None));
        
        repositoryMock
            .Setup(m => m.RemoveEntryAsync(entryId, CancellationToken.None))
            .Verifiable();
                
        await controller.RemoveEntry(entryId);

        repositoryMock.Verify();
    }
    
    [Test]
    public async Task Should_invoke_modification_of_entry()
    {
        var entryId = Guid.NewGuid();
        
        var mockedEntry = Mock.Of<Entry>(x => 
            x.Text == "123" && 
            x.Id == entryId);
        
        var repositoryMock = new Mock<IEntryRepository>();
        var controller = new EntryController(repositoryMock.Object);
        
        repositoryMock.Setup(m => m.SaveEntryAsync(mockedEntry, CancellationToken.None));
        
        repositoryMock
            .Setup(m => m.UpdateEntryTextAsync(entryId, "new text", CancellationToken.None))
            .Verifiable();
                
        await controller.UpdateEntry(entryId, "new text");

        repositoryMock.Verify();
    }
    
    [Test]
    public async Task Should_invoke_addition_of_entry()
    {
        var entryId = Guid.NewGuid();
        
        var mockedEntry = Mock.Of<Entry>(x => 
            x.Text == "123" && 
            x.Id == entryId);
        
        var repositoryMock = new Mock<IEntryRepository>();
        var controller = new EntryController(repositoryMock.Object);
        
        repositoryMock.Setup(m => m.SaveEntryAsync(mockedEntry, CancellationToken.None));
        
        repositoryMock
            .Setup(m => 
                m.SaveEntryAsync(It.Is<Entry>(x => x.Text == mockedEntry.Text), CancellationToken.None))
            .Verifiable();
                
        await controller.AddEntry(new AddEntryCommand(mockedEntry.Text,2, "456"));

        repositoryMock.Verify();
    }
}