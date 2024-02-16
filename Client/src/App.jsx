import './App.css'
import 'bootstrap/dist/css/bootstrap.min.css';
import {
  BrowserRouter,
  Routes,
  Route,
} from 'react-router-dom';
import NotFound from './components/NotFound';
import Header from './components/Header';
import Home from './components/Home';
import UploadMeterReadingsForm from './components/UploadMeterReadingsForm';

function App() {
  return (
    <BrowserRouter>
      <Header />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/upload-meter-readings" element={<UploadMeterReadingsForm />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
