export default function StockBadge({ stock }: { stock: number }) {
    if (stock === 0) return <span className="stock stock-out">Sold out</span>
    if (stock <= 3) return <span className="stock stock-low">Only {stock} left</span>
    return <span className="stock">{stock} in stock</span>
}