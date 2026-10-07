import { useState, type FormEvent } from 'react'
import { getErrorMessage } from '../../api/errors.ts'
import type { ListingRequest, ListingView } from '../../api/generated/Api.ts'
import { useCategories } from '../../api/useCategories.ts'

interface Props {
    initial?: ListingView 
    onSubmit: (data: ListingRequest) => Promise<unknown>
    onCancel: () => void
}

export default function ListingForm({ initial, onSubmit, onCancel }: Props) {
    const { categories } = useCategories()

    const [title, setTitle] = useState(initial?.title ?? '')
    const [categoryId, setCategoryId] = useState(initial?.categoryId ?? 0)
    const [price, setPrice] = useState(initial ? String(initial.price) : '')
    const [stock, setStock] = useState(initial ? String(initial.stock) : '1')
    const [description, setDescription] = useState(initial?.description ?? '')
    const [error, setError] = useState<string | null>(null)
    const [saving, setSaving] = useState(false)

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault()
        setSaving(true)
        setError(null)
        try {
            await onSubmit({
                title,
                categoryId,
                price: Number(price),
                stock: Number(stock),
                description: description.trim() || null,
            })
        } catch (err) {
            setError(getErrorMessage(err))
        } finally {
            setSaving(false)
        }
    }

    return (
        <form className="card form form-wide" onSubmit={handleSubmit}>
            <h2>{initial ? 'Edit listing' : 'New listing'}</h2>

            <div className="field">
                <label htmlFor="lf-title">Title</label>
                <input id="lf-title" className="input" value={title}
                       onChange={(e) => setTitle(e.target.value)} maxLength={100} required />
            </div>

            <div className="field">
                <label htmlFor="lf-category">Category</label>
                <select id="lf-category" className="select" value={categoryId}
                        onChange={(e) => setCategoryId(Number(e.target.value))} required>
                    <option value={0} disabled>Choose a category…</option>
                    {categories.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
            </div>

            <div className="form-row">
                <div className="field">
                    <label htmlFor="lf-price">Price (USD)</label>
                    <input id="lf-price" className="input" type="number" min="0.01" step="0.01"
                           value={price} onChange={(e) => setPrice(e.target.value)} required />
                </div>
                <div className="field">
                    <label htmlFor="lf-stock">Stock</label>
                    <input id="lf-stock" className="input" type="number" min="0" step="1"
                           value={stock} onChange={(e) => setStock(e.target.value)} required />
                </div>
            </div>

            <div className="field">
                <label htmlFor="lf-description">Description (optional)</label>
                <textarea id="lf-description" className="textarea" rows={4} maxLength={1000}
                          value={description} onChange={(e) => setDescription(e.target.value)} />
            </div>

            {error && <p className="form-error">{error}</p>}

            <div className="form-actions">
                <button className="btn btn-primary" disabled={saving || categoryId === 0}>
                    {saving ? 'Saving…' : initial ? 'Save changes' : 'Create listing'}
                </button>
                <button type="button" className="btn btn-ghost" onClick={onCancel}>Cancel</button>
            </div>
        </form>
    )
}