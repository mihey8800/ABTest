import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import { Layout } from './components/Layout'
import { Home } from './pages/Home'
import './App.css';
import './MiniProfiler'


function App() {
  return (
    <Router>
      <Switch>
        <Layout>
            <Route exact path='/' component={Home} />
        </Layout>
      </Switch>
    </Router>
  );
}

export default App;
