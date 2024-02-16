import { render, screen, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom/extend-expect';
import UploadMeterReadingsForm from './UploadMeterReadingsForm';

describe('UploadMeterReadingsForm tests', () => {
    test('renders without errors', () => {
        render(<UploadMeterReadingsForm />);
        expect(screen.getByText('Upload Meter Readings form')).toBeInTheDocument();
    });

    test('displays an error message if no file is selected', () => {
        render(<UploadMeterReadingsForm />);
        fireEvent.submit(screen.getByRole('button', { name: /upload/i }));
        expect(screen.getByText("Please select the file you'd like to upload.")).toBeInTheDocument();
    });

    test('uploads a file and displays success message', async () => {
    });

    test('displays error message on failed upload', async () => {

    });
});
