import * as React from 'react';
import * as ReactDOM from 'react-dom';
import App from './App';
import './index.css';
import registerServiceWorker from './registerServiceWorker';

ReactDOM.render(
  <App data={{ Path: "akka://ClusterSystem/system/sharding" }} />,
  document.getElementById('root') as HTMLElement
);
registerServiceWorker();