import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserSessionComponent } from './user-session/user-session.component';
import { WeatherForecastComponent } from './weather/weather.component';

const routes: Routes = [
  { path: 'user-session', component: UserSessionComponent },
  { path: 'weather', component: WeatherForecastComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
