
import { useState } from 'react'
import { api } from '../api/client.ts'
import type { ListingRequest, ListingView } from '../api/generated/Api.ts'
import { useFetch } from '../api/useFetch.ts'
import ListingForm from '../components/listings/ListingForm.tsx'
import MyListingRow from '../components/listings/MyListingRow.tsx'

type FormMode = { type: 'closed' } | { type: 'create' } | { type: 'edit'; listing: ListingView }

const fetchMine = () => api.listings.listingsMineList()

export default function MyListingsPage() {
  const { data: listings, loading, error, reload } = useFetch(fetchMine)
  const [form, setForm] = useState<FormMode>({ type: 'closed' })

  const closeAndReload = () => {
    setForm({ type: 'closed' })
    reload()
  }

  const create = async (data: ListingRequest) => {
    await api.listings.listingsCreate(data)
    closeAndReload()
  }

  const update = (id: number) => async (data: ListingRequest) => {
    await api.listings.listingsUpdate(id, data)
    closeAndReload()
  }

  const active = listings?.filter((l) => l.isActive) ?? []
  const inactive = listings?.filter((l) => !l.isActive) ?? []

  return (
      <section className="container page">
        <div className="page-head">
          <h1 className="page-title">My shop</h1>
          {form.type === 'closed' && (
              <button className="btn btn-primary" onClick={() => setForm({ type: 'create' })}>
                + New listing
              </button>
          )}
        </div>

        {form.type === 'create' && (
            <ListingForm onSubmit={create} onCancel={() => setForm({ type: 'closed' })} />
        )}
        {form.type === 'edit' && (
            <ListingForm
                key={form.listing.id} // jiný listing → nový formulář s jeho daty
                initial={form.listing}
                onSubmit={update(form.listing.id)}
                onCancel={() => setForm({ type: 'closed' })}
            />
        )}

        {loading && <p className="muted">Loading…</p>}
        {error && <p className="alert alert-error">{error}</p>}

        {!loading && listings && (
            active.length === 0 && inactive.length === 0 ? (
                <div className="card empty-state">You are not selling anything yet.</div>
            ) : (
                <ul className="card item-list">
                  {[...active, ...inactive].map((l) => (
                      <MyListingRow
                          key={`${l.id}-${l.stock}`} // po změně skladu se řádek "resetuje" na novou hodnotu
                          listing={l}
                          onEdit={() => setForm({ type: 'edit', listing: l })}
                          onChanged={reload}
                      />
                  ))}
                </ul>
            )
        )}
      </section>
  )
}