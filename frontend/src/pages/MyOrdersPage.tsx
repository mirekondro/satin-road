import { useState } from 'react'
import { api } from '../api/client.ts'
import { useFetch } from '../api/useFetch.ts'
import OrderRow from '../components/orders/OrderRow.tsx'

// Defined outside the component → same function every render, so useFetch doesn't refetch in a loop
const fetchBought = () => api.orders.ordersMineList()
const fetchSold = () => api.orders.ordersSalesList()

type Tab = 'bought' | 'sold'

export default function MyOrdersPage() {
  const [tab, setTab] = useState<Tab>('bought')
  const { data: orders, loading, error } = useFetch(tab === 'bought' ? fetchBought : fetchSold)

  return (
      <section className="container page">
        <h1 className="page-title">My orders</h1>

        <div className="chips">
          <button
              type="button"
              className={`chip ${tab === 'bought' ? 'active' : ''}`}
              aria-pressed={tab === 'bought'}
              onClick={() => setTab('bought')}
          >
            Purchases
          </button>
          <button
              type="button"
              className={`chip ${tab === 'sold' ? 'active' : ''}`}
              aria-pressed={tab === 'sold'}
              onClick={() => setTab('sold')}
          >
            Sales
          </button>
        </div>

        {loading && <p className="muted">Loading…</p>}
        {error && <p className="alert alert-error">{error}</p>}

        {!loading && !error && orders && (
            orders.length === 0 ? (
                <div className="card empty-state">
                  {tab === 'bought' ? 'You have not bought anything yet.' : 'Nobody has bought from you yet.'}
                </div>
            ) : (
                <ul className="card item-list">
                  {orders.map((o) => <OrderRow key={o.id} order={o} mode={tab} />)}
                </ul>
            )
        )}
      </section>
  )
}