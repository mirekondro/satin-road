import { useState } from 'react'
import { Link } from 'react-router'
import { api } from '../../api/client.ts'
import { getErrorMessage } from '../../api/errors.ts'
import type { ListingView } from '../../api/generated/Api.ts'
import { formatPrice } from '../../utils/format.ts'

interface Props {
    listing: ListingView
    onEdit: () => void
    onChanged: () => void
}

export default function MyListingRow({ listing, onEdit, onChanged }: Props) {
    const [stock, setStock] = useState(String(listing.stock))
    const [error, setError] = useState<string | null>(null)
    const [busy, setBusy] = useState(false)

    const stockChanged = Number(stock) !== listing.stock

    const run = async (action: () => Promise<unknown>) => {
        setBusy(true)
        setError(null)
        try {
            await action()
            onChanged()
        } catch (e) {
            setError(getErrorMessage(e))
        } finally {
            setBusy(false)
        }
    }

    const saveStock = () =>
        run(() => api.listings.listingsStockPartialUpdate(listing.id, { stock: Number(stock) }))

    const deactivate = () => {
        if (!window.confirm(`Remove "${listing.title}" from sale?`)) return
        void run(() => api.listings.listingsDelete(listing.id))
    }

    if (!listing.isActive) {
        return (
            <li className="item-row item-row-inactive">
                <div className="item-main">
                    <span className="item-title">{listing.title}</span>
                    <span className="muted"> · removed from sale</span>
                </div>
            </li>
        )
    }

    return (
        <li className="item-row">
            <div className="item-main">
                <Link to={`/listings/${listing.id}`} className="item-title">{listing.title}</Link>
                <p className="muted item-meta">
                    {listing.categoryName} · {formatPrice(listing.price)}
                </p>
                {error && <p className="form-error">{error}</p>}
            </div>

            <div className="item-actions">
                <label className="stock-editor">
                    <span className="sr-only">Stock for {listing.title}</span>
                    <input
                        className="input input-sm"
                        type="number"
                        min="0"
                        value={stock}
                        onChange={(e) => setStock(e.target.value)}
                        onKeyDown={(e) => { if (e.key === 'Enter' && stockChanged) void saveStock() }}
                    />
                </label>
                {stockChanged && (
                    <button className="btn btn-primary btn-sm" onClick={() => void saveStock()} disabled={busy}>
                        Save stock
                    </button>
                )}
                <button className="btn btn-ghost btn-sm" onClick={onEdit} disabled={busy}>Edit</button>
                <button className="btn btn-danger btn-sm" onClick={deactivate} disabled={busy}>Remove</button>
            </div>
        </li>
    )
}