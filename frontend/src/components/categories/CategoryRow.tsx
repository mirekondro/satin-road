import { useState } from 'react'
import { api } from '../../api/client.ts'
import { getErrorMessage } from '../../api/errors.ts'
import type { CategoryDto } from '../../api/generated/Api.ts'

interface Props {
    category: CategoryDto
    onChanged: () => void 
}


export default function CategoryRow({ category, onChanged }: Props) {
    const [editing, setEditing] = useState(false)
    const [name, setName] = useState(category.name)
    const [error, setError] = useState<string | null>(null)
    const [busy, setBusy] = useState(false)

    
    const run = async (action: () => Promise<unknown>) => {
        setBusy(true)
        setError(null)
        try {
            await action()
            setEditing(false)
            onChanged()
        } catch (e) {
            setError(getErrorMessage(e))
        } finally {
            setBusy(false)
        }
    }

    const save = () => run(() => api.categories.categoriesUpdate(category.id, { name }))

    const remove = () => {
        if (!window.confirm(`Delete category "${category.name}"?`)) return
        void run(() => api.categories.categoriesDelete(category.id))
    }

    const cancel = () => {
        setName(category.name)
        setError(null)
        setEditing(false)
    }

    return (
        <li className="item-row">
            <div className="item-main">
                {editing ? (
                    <input
                        className="input"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        onKeyDown={(e) => {
                            if (e.key === 'Enter') void save()
                            if (e.key === 'Escape') cancel()
                        }}
                        maxLength={50}
                        autoFocus
                        aria-label="Category name"
                    />
                ) : (
                    <span className="item-title">{category.name}</span>
                )}
                {error && <p className="form-error">{error}</p>}
            </div>

            <div className="item-actions">
                {editing ? (
                    <>
                        <button className="btn btn-primary btn-sm" onClick={() => void save()} disabled={busy}>
                            Save
                        </button>
                        <button className="btn btn-ghost btn-sm" onClick={cancel} disabled={busy}>
                            Cancel
                        </button>
                    </>
                ) : (
                    <>
                        <button className="btn btn-ghost btn-sm" onClick={() => setEditing(true)} disabled={busy}>
                            Rename
                        </button>
                        <button className="btn btn-danger btn-sm" onClick={remove} disabled={busy}>
                            Delete
                        </button>
                    </>
                )}
            </div>
        </li>
    )
}