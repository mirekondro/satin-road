import { useParams } from 'react-router'
import Placeholder from '../components/Placeholder.tsx'

export default function ListingDetailPage() {
  const { id } = useParams()
  return <Placeholder title={`Listing #${id}`}>Listing detail + Buy button.</Placeholder>
}
