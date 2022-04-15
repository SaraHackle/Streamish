import React from "react";
import { Switch, Route } from "react-router-dom";
import VideoList from "./VideoList";
import VideoForm from "./VideoForm";
import VideoDetails from "./VideoDetails";

const ApplicationViews = () => {
  return (
    <Switch>
      <Route path="/" exact>
        <VideoList />
      </Route>

      <Route path="/videos/add" exact>
        <VideoForm />
      </Route>

      <Route path="/videos/:id" >
          <VideoDetails/>
      </Route>
    </Switch>
  );
};

export default ApplicationViews;