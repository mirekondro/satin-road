import { Link } from 'react-router'
import type { OrderView } from '../../api/generated/Api.ts'
import { formatPrice } from '../../utils/format.ts'

const dateFormatter = new Intl.DateTimeFormat('en-GB', { dateStyle: 'medium', timeStyle: 'short' })

interface Props {
    order: OrderView
    mode: 'bought' | 'sold'
}

export default function OrderRow({ order, mode }: Props) {
    const who = mode === 'bought' ? `from @${order.vendorName}` : `to @${order.buyerName}`

    return (
        <li className="item-row">
            <div className="item-main">
                <Link to={`/listings/${order.listingId}`} className="item-title">
                    {order.listingTitle}
                </Link>
                <p className="muted">
                    {who} · {dateFormatter.format(new Date(order.createdAt))}
                </p>
            </div>

            <div className="order-amount">
                <span className="muted">{order.quantity}× {formatPrice(order.unitPrice)}</span>
                {order.discountApplied && <span className="badge">−20%</span>}
                <strong>{formatPrice(order.total)}</strong>
            </div>
        </li>
    )
}