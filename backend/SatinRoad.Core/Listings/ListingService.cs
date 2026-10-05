using SatinRoad.Core.Common;
using SatinRoad.Core.Entities;

namespace SatinRoad.Core.Listings;

public class ListingService(IListingRepository repo)
{
    public const int TitleMinLength = 3;
    public const int TitleMaxLength = 100;
    public const int DescriptionMaxLength = 1000;
    public const decimal MaxPrice = 1_000_000m;

    // ---------- Čtení ----------

    public Task<List<ListingView>> GetActiveAsync(int? categoryId = null) =>
        repo.GetActiveAsync(categoryId);

    public Task<List<ListingView>> GetMineAsync(int vendorId) =>
        repo.GetByVendorAsync(vendorId);

    public async Task<ListingView> GetByIdAsync(int id)
    {
        var view = await repo.GetViewByIdAsync(id);

        // Deaktivovaný listing se navenek tváří, že neexistuje
        if (view is null || !view.IsActive)
            throw new NotFoundException($"Listing {id} not found.");

        return view;
    }

    // ---------- Zápis ----------

    public async Task<ListingView> CreateAsync(int vendorId, ListingInput input)
    {
        var (title, description) = await ValidateAsync(input);

        var listing = await repo.AddAsync(new Listing
        {
            VendorId = vendorId,
            CategoryId = input.CategoryId,
            Title = title,
            Description = description,
            Price = input.Price,
            Stock = input.Stock,
            IsActive = true,
        });

        return await GetViewAsync(listing.Id);
    }

    public async Task<ListingView> UpdateAsync(int vendorId, int listingId, ListingInput input)
    {
        var listing = await GetOwnedAsync(vendorId, listingId); // nejdřív 404/403…
        var (title, description) = await ValidateAsync(input);  // …pak 400

        listing.CategoryId = input.CategoryId;
        listing.Title = title;
        listing.Description = description;
        listing.Price = input.Price;
        listing.Stock = input.Stock;

        await repo.UpdateAsync(listing);
        return await GetViewAsync(listing.Id);
    }

    public async Task<ListingView> UpdateStockAsync(int vendorId, int listingId, int stock)
    {
        var listing = await GetOwnedAsync(vendorId, listingId);

        if (stock < 0)
            throw new ValidationException("Stock cannot be negative.");

        listing.Stock = stock;
        await repo.UpdateAsync(listing);
        return await GetViewAsync(listing.Id);
    }

    public async Task DeactivateAsync(int vendorId, int listingId)
    {
        var listing = await GetOwnedAsync(vendorId, listingId);

        listing.IsActive = false;
        await repo.UpdateAsync(listing);
    }

    // ---------- Pomocné metody ----------

    private async Task<Listing> GetOwnedAsync(int vendorId, int listingId)
    {
        var listing = await repo.GetByIdAsync(listingId);

        if (listing is null || !listing.IsActive)
            throw new NotFoundException($"Listing {listingId} not found.");

        if (listing.VendorId != vendorId)
            throw new ForbiddenException("You can only manage your own listings.");

        return listing;
    }

    private async Task<ListingView> GetViewAsync(int id) =>
        await repo.GetViewByIdAsync(id)
        ?? throw new NotFoundException($"Listing {id} not found.");

    private async Task<(string Title, string? Description)> ValidateAsync(ListingInput input)
    {
        var title = input.Title?.Trim() ?? "";
        if (title.Length < TitleMinLength || title.Length > TitleMaxLength)
            throw new ValidationException(
                $"Title must be {TitleMinLength}-{TitleMaxLength} characters long.");

        var description = string.IsNullOrWhiteSpace(input.Description)
            ? null
            : input.Description.Trim();
        if (description is not null && description.Length > DescriptionMaxLength)
            throw new ValidationException(
                $"Description can be at most {DescriptionMaxLength} characters long.");

        if (input.Price <= 0 || input.Price > MaxPrice)
            throw new ValidationException($"Price must be between 0.01 and {MaxPrice:N0}.");

        if (decimal.Round(input.Price, 2) != input.Price)
            throw new ValidationException("Price can have at most 2 decimal places.");

        if (input.Stock < 0)
            throw new ValidationException("Stock cannot be negative.");

        if (!await repo.CategoryExistsAsync(input.CategoryId))
            throw new ValidationException($"Category {input.CategoryId} does not exist.");

        return (title, description);
    }
}