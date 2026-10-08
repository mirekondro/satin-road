import { useCallback } from 'react'
import { useSearchParams } from 'react-router'
import { api } from '../api/client.ts'
import { useFetch } from '../api/useFetch.ts'
import CategoryFilter from '../components/categories/CategoryFilter.tsx'
import ListingCard from '../components/listings/ListingCard.tsx'

export default function ListingsPage() {
  const [searchParams] = useSearchParams()
  const categoryParam = searchParams.get('category')
  const categoryId = categoryParam ? Number(categoryParam) : undefined

  // When the category changes, a new fetcher is created → useFetch loads again
  const fetchListings = useCallback(
    () => api.listings.listingsList({ categoryId }),
    [categoryId],
  )
  const { data: listings, loading, error } = useFetch(fetchListings)

  return (
    <section className="container page">
      <h1 className="page-title">Listings</h1>
      <CategoryFilter />

      {loading && (
        <div className="grid">
          {[1, 2, 3].map((i) => <div key={i} className="card skeleton" aria-hidden />)}
        </div>
      )}
      {error && <p className="alert alert-error">{error}</p>}

      {!loading && listings && (
        listings.length === 0 ? (
          <div className="card empty-state">Nothing here yet. The goods are hiding.</div>
        ) : (
          <div className="grid">
            {listings.map((l) => <ListingCard key={l.id} listing={l} />)}
          </div>
        )
      )}
    </section>
  )
}
