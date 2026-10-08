import { useSearchParams } from 'react-router'
import { useCategories } from '../../api/useCategories.ts'

// Filtr podle kategorie. Vybraná kategorie je v URL (?category=2),
// takže jde odkaz sdílet a funguje tlačítko Zpět.
export default function CategoryFilter() {
    const { categories } = useCategories()
    const [searchParams, setSearchParams] = useSearchParams()
    const selected = searchParams.get('category')

    const select = (id: number | null) => {
        const next = new URLSearchParams(searchParams)
        if (id === null) next.delete('category')
        else next.set('category', String(id))
        setSearchParams(next)
    }

    return (
        <div className="chips" role="group" aria-label="Filter by category">
            <button
                className={selected === null ? 'chip active' : 'chip'}
                aria-pressed={selected === null}
                onClick={() => select(null)}
            >
                All
            </button>
            {categories.map((c) => {
                const active = selected === String(c.id)
                return (
                    <button
                        key={c.id}
                        className={active ? 'chip active' : 'chip'}
                        aria-pressed={active}
                        onClick={() => select(c.id)}
                    >
                        {c.name}
                    </button>
                )
            })}
        </div>
    )
}