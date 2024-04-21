import { useRef, useState } from 'react';
import './App.css';
import axios from 'axios';
import FormInput from './components/FormInput';

function App() {
    const [userValues, setUserValues] = useState({
        firstName: "",
        lastName: "",
        username: "",
        email: "",
        password: "",
    });

    const [roleValue, setRoleValue] = useState("");


    const inputs = [
        {
            id: 1,
            name: "firstName",
            type: "text",
            placeholder: "e.g. Stephen",
            errorMessage: "First Name should be at least 7 characters and shouldn't include any special character!",
            label: "First Name",
            pattern: "^[A-Za-z]{2,40}$",
            required: true,
        },
        {
            id: 2,
            name: "lastName",
            type: "text",
            placeholder: "e.g. Stephen",
            errorMessage: "Last Name should be at least 7 characters and shouldn't include any special character!",
            label: "Last Name",
            pattern: "^[A-Za-z]{2,40}$",
            required: true,
        },
        {
            id: 3,
            name: "username",
            type: "text",
            placeholder: "e.g. stephen.king",
            errorMessage: "Username should be 3-16 characters and shouldn't include any special character!",
            label: "Username",
            pattern: "^[A-Za-z0-9](?=.*[0-9])(?=.*[!@#$%^&*]){3,16}$",
            required: true,
        },
        {
            id: 4,
            name: "email",
            type: "email",
            placeholder: "e.g. stephen.king@gmail.com",
            errorMessage: "It should be a valid email address!",
            pattern:".+@gmail\.com|.+@email\.com|.+@yahoo\.com",
            label: "Email",
            required: true,
        },
        {
            id: 5,
            name: "password",
            type: "password",
            placeholder: "password",
            errorMessage: "Password should have at least 6 characters and include at least 1 lowercase, 1 uppercase, 1 number and 1 non-aplhabetical character!",
            label: "Password",
            pattern: '^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*.><_-])[a-zA-Z0-9!@#$%^&*]{6,}$',
            required: true,
        },
        {
            id: 6,
            name: "confirmPassword",
            type: "password",
            placeholder: "confirm password",
            errorMessage: "Password confirmation does not match!",
            pattern: userValues.password,
            label: "Confirm Password",
            required: true,
        },
       
    ];

    const onChange = (e) => {
        const { name, value } = e.target;
        setUserValues({ ...userValues, [name]: value });    
    };

    const handleSubmit = async (e) => {
        console.log(userValues);
        e.preventDefault();
        try {
            const response = await axios.post('https://localhost:7258/api/Authentication/registration', userValues);
            console.log(response);
        } catch (error) {
            console.error('Error:', error);
        }
    };

    return (
        <div className="App">
            <form onSubmit={handleSubmit}>
                <h1>Sign Up</h1>
                {inputs.map((input) => (
                    <FormInput key={input.id} {...input} value={input.name === 'role' ? roleValue : userValues[input.name]} onChange={onChange}/>
                ))}
                <button>Submit</button>
            </form>
        </div>
    );
}

export default App;
