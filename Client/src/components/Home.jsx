import Nav from 'react-bootstrap/Nav';

const linkStyles = {
    color: 'hotpink',
};

function Home() {
    return (
        <div>
            <h1>Welcome</h1>
            <h5 className="mt-5">Click <Nav.Link href="/upload-meter-readings" style={linkStyles}>here</Nav.Link> to Upload Meter Readings CSV file</h5>
        </div>
    );
}

export default Home;