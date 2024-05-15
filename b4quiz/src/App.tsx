import './App.css'
import React from 'react'
import QuizCreate from './pages/QuizCreate'
import QuizCreateFoo from './pages/QuizCreateFoo'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'

import Navbar from './components/Navbar'
import HomePage from './pages/HomePage'

import TagsPage from './pages/TagsPage'
import QuizzesPage from './pages/QuizzesPage'
import FormGetId from './pages/QuizGet'
import ShowInfo from './components/ShowQuizInfo'
import AIUse from './AIUse'

function App(): JSX.Element {
  return (
    <Router>
      <div className="bg-[#879693] min-h-screen">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/create-quiz" element={<QuizCreate />} />
          <Route path="/create-foo" element={<QuizCreateFoo />} />

          <Route path="/my-quizzes" element={<HomePage />} />
          <Route path="/tags" element={<TagsPage />} />
          <Route path="/aiuse" element={<AIUse />} />
          <Route path="/quiz/:id" element={<FormGetId />} />
          <Route path="/profile" element={<HomePage />} />
          <Route path="/quizzes" element={<QuizzesPage />} />
        </Routes>
      </div>
    </Router>
  )
}

export default App
