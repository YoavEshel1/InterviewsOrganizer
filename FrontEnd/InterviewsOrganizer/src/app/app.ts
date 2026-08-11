import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from "./structure/header/header";
import { Footer } from "./structure/footer/footer";
import { LoginArea } from "./Features/login/login-area/login-area";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, Footer, LoginArea],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App { 
  pageTitle = 'Interviews Organizer';
}
