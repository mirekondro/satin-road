import { Link } from 'react-router'
import type { ListingView } from '../../api/generated/Api.ts'
import { formatPrice } from '../../utils/format.ts'
import StockBadge from './StockBadge.tsx'

export default function ListingCard({ listing }: { listing: ListingView }) {
    return (
        <Link to={`/listings/${listing.id}`} className="card listing-card">
            <div className="listing-card-top">
                <span className="badge">{listing.categoryName}</span>
                <StockBadge stock={listing.stock} />
            </div>
            <h2 className="listing-card-title">{listing.title}</h2>
            <p className="muted listing-card-vendor">by @{listing.vendorName}</p>
            <p className="listing-price">{formatPrice(listing.price)}</p>
        </Link>
    )
}