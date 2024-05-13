import React, { useState, useEffect } from 'react';
import Navbar from '../components/Navbar';

const TagsPage = () => {
    const [tags, setTags] = useState([]);
    const [updateName, setUpdateName] = useState('');
    const [newTagName, setNewTagName] = useState('');

    useEffect(() => {
        const fetchTags = async () => {
            try {
                const response = await fetch('http://localhost:5276/api/tags');
                if (!response.ok) {
                    throw new Error('Failed to fetch tags');
                }
                const data = await response.json();
                setTags(data);
            } catch (error) {
                console.error('Error fetching tags:', error);
            }
        };

        fetchTags();
    }, []);

    const handleUpdate = async (tagId) => {
        try {
            const response = await fetch(`http://localhost:5276/api/tags/${tagId}`, {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ Name: updateName })
            });

            if (!response.ok) {
                throw new Error('Failed to update tag');
            }

            const updatedTags = tags.map(tag => {
                if (tag.id === tagId) {
                    return { ...tag, name: updateName };
                }
                return tag;
            });
            setTags(updatedTags);
        } catch (error) {
            console.error('Error updating tag:', error);
        }
    };

    const handleDelete = async (tagId) => {
        try {
            const response = await fetch(`http://localhost:5276/api/tags/${tagId}`, {
                method: 'DELETE'
            });

            if (!response.ok) {
                throw new Error('Failed to delete tag');
            }

            const updatedTags = tags.filter(tag => tag.id !== tagId);
            setTags(updatedTags);
        } catch (error) {
            console.error('Error deleting tag:', error);
        }
    };

    const handleCreate = async () => {
        try {
            const response = await fetch('http://localhost:5276/api/tags', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ Name: newTagName })
            });

            if (!response.ok) {
                throw new Error('Failed to create tag');
            }

            const createdTagId = await response.text();

            setTags([...tags, { id: createdTagId, name: newTagName }]);
            setNewTagName('');
        } catch (error) {
            console.error('Error creating tag:', error);
        }
    };

    return (
        <div>
            <Navbar />
            <div className='flex flex-col items-center'>
                <h1 className='text-[#efd7cf] text-4xl font-bold mt-6'>Tags</h1>
                <div className="flex justify-end mb-4 mt-8">
                    <input type="text" value={newTagName} onChange={e => setNewTagName(e.target.value)} className="border border-gray-300 px-2 py-1 rounded-md mr-2" />
                    <button onClick={handleCreate} className="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded bg-[#436e6f] hover:bg-[#1c4e4f]" style={{ transition: 'background-color 0.3s ease'}}>Create Tag</button>
                </div>
                <ul className="mt-8 grid md:grid-cols-2 lg:grid-cols-4 gap-4">
                    {tags.map(tag => (
                        <li key={tag.id} className="flex flex-col items-center justify-center bg-[#f7ebe7] text-[#0a2d2e] py-2 px-4 rounded-lg shadow-md transition-colors duration-200">
                            <div className="mb-4 text-2xl text-[#1c4e4f]">
                            <span>{tag.name}</span>
                            </div>
                            <div className="mt-2">
                                <input type="text" value={updateName} onChange={e => setUpdateName(e.target.value)} className="border border-gray-300 px-2 py-1 rounded-md mr-2" />
                                <button onClick={() => handleUpdate(tag.id)} className="text-white font-bold py-2 px-4 rounded bg-[#436e6f] hover:bg-[#1c4e4f]" style={{transition: 'background-color 0.3s ease'}}>Update</button>
                            </div>
                            <div className="mt-2">
                                <button onClick={() => handleDelete(tag.id)} className="text-white font-bold py-2 px-4 rounded bg-[#deae9f] hover:bg-[#a49e97]" style={{transition: 'background-color 0.3s ease'}}>Delete</button>
                            </div>   
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};

export default TagsPage;
