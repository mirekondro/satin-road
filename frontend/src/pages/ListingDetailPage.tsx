import { useCallback } from 'react'
import { Link, useParams } from 'react-router'
import { api } from '../api/client.ts'
import { useFetch } from '../api/useFetch.ts'
import StockBadge from '../components/listings/StockBadge.tsx'
import { formatPrice } from '../utils/format.ts'

export default function ListingDetailPage() {
  const id = Number(useParams().id)

  const fetchListing = useCallback(() => api.listings.listingsDetail(id), [id])
  const { data: listing, loading, error } = useFetch(fetchListing)

  if (loading) return <section className="container page"><p className="muted">Loading…</p></section>

  if (error || !listing) {
    return (
        <section className="container page">
          <p className="alert alert-error">{error ?? 'Listing not found.'}</p>
          <Link to="/listings" className="btn btn-ghost">← Back to listings</Link>
        </section>
    )
  }

  return (
      <section className="container page">
        <Link to="/listings" className="back-link">← Back to listings</Link>

        <div className="listing-detail card">
          <div className="listing-card-top">
            <Link to={`/listings?category=${listing.categoryId}`} className="badge">
              {listing.categoryName}
            </Link>
            <StockBadge stock={listing.stock} />
          </div>

          <h1 className="listing-detail-title">{listing.title}</h1>
          <p className="muted">Sold by @{listing.vendorName}</p>

          {listing.description && <p className="listing-description">{listing.description}</p>}

          <div className="listing-buy">
            <span className="listing-price listing-price-lg">{formatPrice(listing.price)}</span>
            {/* Purchase button */}
            <button className="btn btn-primary" disabled={listing.stock === 0}>
              {listing.stock === 0 ? 'Sold out' : 'Buy'}
            </button>
          </div>
        </div>
      </section>
  )
}