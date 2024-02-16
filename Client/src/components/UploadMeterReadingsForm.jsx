import { useState } from 'react';
import { Form, Button } from 'react-bootstrap';

function UploadMeterReadingsForm() {
    const [selectedFile, setSelectedFile] = useState(null);
    const [message, setMessage] = useState('');
    const [isUploading, setIsUploading] = useState(false);
    const [result, setResult] = useState(null);

    const onChangeHandler = (e) => {
        const file = e.target.files[0];
        setSelectedFile(file);
        setMessage('');
    };

    const onSubmitHandler = async (e) => {
        e.preventDefault();

        const formData = new FormData();
        formData.append('file', selectedFile);

        if (selectedFile) {
            setIsUploading(true);
            setMessage('Your file is being uploaded...');
            setResult('');

            fetch('https://localhost:7264/meter-reading-uploads', {
                method: 'POST',
                body: formData,
            })
                .then(async response => {
                    if (!response.ok) {
                        const errorData = await response.json();
                        throw new Error(JSON.stringify(errorData));
                    }
                    return response.json();
                })
                .then(data => {
                    setResult(data);
                    setIsUploading(false);
                    setMessage('');
                })
                .catch(error => {
                    setIsUploading(false);
                    setResult('');

                    const errorData = JSON.parse(error.message);
                    if (errorData.validationDetails) {
                        const errorMessage = Object.keys(errorData.errors)
                            .map(key => <p>{`${key}: ${errorData.errors[key].join(', ')}`}</p>);

                        setMessage(errorMessage);
                    } else {
                        const errorMessage = errorData.map(x => <p>{x}</p>)
                        setMessage(errorMessage);
                    }
                });
        } else {
            setMessage('Please select the file you\'d like to upload.');
        }
    };

    return (
        <>
            <h2 className="mb-5">Upload Meter Readings form</h2>
            <Form onSubmit={onSubmitHandler}>
                <Form.Group controlId="formFile" className="mb-3">
                    <Form.Label>Select a CSV file:</Form.Label>
                    <Form.Control type="file" onChange={onChangeHandler} />
                </Form.Group>
                <Button variant="primary" type="submit">
                    Upload
                </Button>
            </Form>
            {message && <div className={`mt-3 ${isUploading ? "text-success" : "text-danger"}`}>{message}</div>}
            {result && <h5 className="mt-3">Successful meter readings: {result.successReadings}, failed meter readings {result.failedReadings}</h5>}
        </>
    );
}

export default UploadMeterReadingsForm;

