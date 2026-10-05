import { useSearchParams } from 'react-router'
import CategoryFilter from '../components/categories/CategoryFilter.tsx'

export default function ListingsPage() {
  const [searchParams] = useSearchParams()
  const categoryId = searchParams.get('category')

  return (
      <section className="container page">
        <h1 className="page-title">Listings</h1>
        <CategoryFilter />

        {/* TODO (Listings): load listings and filter by categoryId*/}
        <div className="card empty-state">
          {categoryId ? `Listings in category #${categoryId} – coming soon.` : 'Listings – coming soon.'}
        </div>
      </section>
  )
}