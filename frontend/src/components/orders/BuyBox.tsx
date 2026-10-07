import { useState } from 'react'
import { Link, useLocation } from 'react-router'
import { api } from '../../api/client.ts'
import { getErrorMessage } from '../../api/errors.ts'
import type { ListingView, PlacedOrderResponse } from '../../api/generated/Api.ts'
import { useAuth } from '../../auth/useAuth.ts'
import { formatPrice } from '../../utils/format.ts'

const MAX_QUANTITY = 100 // same limit as OrderService.MaxQuantity

interface Props {
    listing: ListingView
    stock: number                         // current stock (goes down after each purchase)
    onBought: (quantity: number) => void
}

export default function BuyBox({ listing, stock, onBought }: Props) {
    const { user } = useAuth()
    const location = useLocation()
    const [quantity, setQuantity] = useState(1)
    const [buying, setBuying] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [result, setResult] = useState<PlacedOrderResponse | null>(null)

    // Not logged in → send to login and come back here afterwards
    if (!user) {
        return (
            <Link to="/login" state={{ from: location.pathname }} className="btn btn-primary">
                Log in to buy
            </Link>
        )
    }

    if (user.id === listing.vendorId) {
        return <span className="muted">This is your listing.</span>
    }

    // Hard story #12 – the buyer turned out to be FBI
    if (result?.vendorShutDown) {
        return (
            <div className="alert alert-error">
                <strong>Busted.</strong> You were an undercover FBI agent. @{listing.vendorName} has been
                shut down and all their listings were removed. <Link to="/listings">Back to listings</Link>
            </div>
        )
    }

    const maxQuantity = Math.min(stock, MAX_QUANTITY)

    const handleBuy = async () => {
        setBuying(true)
        setError(null)
        setResult(null)
        try {
            const order = await api.orders.ordersCreate({ listingId: listing.id, quantity })
            setResult(order)
            if (!order.vendorShutDown) onBought(order.quantity)
            setQuantity(1)
        } catch (e) {
            setError(getErrorMessage(e)) // e.g. "Not enough stock. Only 2 left."
        } finally {
            setBuying(false)
        }
    }

    return (
        <div className="buy-box">
            <div className="buy-controls">
                <label htmlFor="quantity" className="sr-only">Quantity</label>
                <input
                    id="quantity"
                    type="number"
                    className="input input-sm"
                    min={1}
                    max={maxQuantity}
                    value={quantity}
                    disabled={stock === 0 || buying}
                    onChange={(e) =>
                        setQuantity(Math.max(1, Math.min(maxQuantity, Number(e.target.value) || 1)))
                    }
                />
                <button className="btn btn-primary" onClick={handleBuy} disabled={stock === 0 || buying}>
                    {stock === 0 ? 'Sold out' : buying ? 'Buying…' : `Buy for ${formatPrice(listing.price * quantity)}`}
                </button>
            </div>

            {error && <p className="alert alert-error">{error}</p>}

            {result && (
                <p className="alert alert-success">
                    Order #{result.id} placed: {result.quantity}× for {formatPrice(result.total)}.
                    {result.discountApplied && ' Loyalty discount −20% applied!'}
                </p>
            )}
        </div>
    )
}