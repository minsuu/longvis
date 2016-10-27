---
title: Long Time Process Data Visualization
subtitle: 2016 Fall, 창의적종합설계1 U조 project보고서
author:
- 김민수
- 조용훤
toc: 1
toc-depth: 2
---

# Abstract & Motivation

해당 프로젝트에서는 Data Reduction Algorithm을 중심으로 해서 몇 가지의 Algorithm과 Graphic Visualization 방식을 선택하여, 데이터 시각화를 보다 빠르고 효율적으로 할 수 있는 Windows Application을 만드는 것을 목표로 한다. 

구체적으로 우리가 설계한 Application의 기능을 한 문장으로 정리하면 다음과 같다. 그 핵심적인 기능은 각 웨이퍼의 센서를 통해서 수집된 수십만 단위의 이상의 빅 데이터를 가져와 Database에 저장하고 그것을 요구에 맞게 불러와 Graph로 그려내는 것이다. 

위 문장에서 요구에 맞게 불러온다는 부분이 중요하다. 우리는 삼성전자 생산기술연구소의 담당자와의 협의를 통해서 스펙을 구체화했고 몇 가지 쟁점에 대해서 정리를 할 수 있었다. 

그 결과로 우리가 목표로 하는 Application에는 단순히 빅 데이터를 저장하고 불러와 시각화하는 것을 넘어서 그 불러오는 데이터의 범위조절 기능, 사용자 편의성을 위한 확대와 축소 기능을 추가로 담기로 하였다.    

결국 이 모든 기능에 있어 가장 큰 쟁점이 되는 것은 다양한 기능보다는 정확히 필요한 기능에 따르는 속도와 정확성이다. 사측의 요구사항은 명백했고 우리는 항상 그 부분에 신경을 써서 생각했다. 

# Introduction

## 과제목표

우리의 Graphic Visualization Application은 반도체 생산과정에서 수집된 데이터를 수신하여 각각의 웨이퍼가 어떤 상태인지 확인하기 위한 용도이다. 그렇기 때문에 Visualization 과정에서 반드시 정확성이 요구된다.  .Net 사용자를 위해서 제공되는 Cross-Platform Plotting library인 OxyPlot은 오래되고 다양한 사용경험이 보고되어 있어 그 신뢰성을 높은 도구로 우리가 Visualization에 사용하기에 적합한 것이었다. 

이를 통해서 우리는 Visualization의 구체적인 작동에 초점을 맞추기 보다는 수집된 데이터를 가공하는 것에 좀 더 많은 노력을 기울일 수 있게 되었고 사측 담당자와의 면담의 결과 이것이 맞는 방향임은 명백했다. 

비록 우리에게 제공되지는 않았지만 담당자에 따르면 사측에서 기존에 사용하고 있는 Application은 Visualization의 정확성에 대한 이슈에서는 자유로운 것이었고 문제가 되는 것은 그 속도였다. 

바로 이 속도를 결정짓는 가장 큰 요인은 무엇일까? 관련이 있을 법한 여러 가지 이슈들을 찾아보다가 우리가 발견한 것은 Data Reduction Algorithm이었다. 이것에 주목한 것은 아주 단순한 발상에서부터였다. 바로 그리는 데에 사용하는 데이터가 적다면 빠를 것이라는 점이다. 

또한 쟁점이 되는 것은 Graphic Visualization을 수행 했을 때에 나오는 그래프의 패턴이다. 역시 담당자와의 협의를 통해서 알게 된 것으로 실제로 사측에서는 본래 나와야하는 그래프의 패턴과 새로 그려져 나온 그래프의 패턴을 비교하여 웨이퍼의 이상 작동을 찾고 있다 한다. 

이 같은 과정을 통해서 우리는 문제를 정리했다. 그래프는 정확해야 하고 문제가 되는 것은 그 패턴이니 Data Reduction Algorithm을 통해서 수집한 데이터를 가공하여 눈으로 봤을 때 동일한 패턴이 유지되면 되는 것이다. 이것이 확대, 축소 기능을 지원해야 함은 물론이다. 

Algorithm의 효율성에 의해서 실제 수행 시간이 미미하게만 감소하거나 반대로 굉장히 비효율적이라서 오히려 수행 시간이 증가할 수도 있겠지만 적절히만 적용된다면 비약적인 수행 시간 감소가 있을 것은 분명하다. 

## 보고서의 구성

# Background Study

## Overview

본 project는 서버와 클라이언트 두 개의 application으로 나뉘어있고, 이것은 모두 Windows환경에서 실행된다. 따라서 두 application 모두 .NET Framework를 통하여 작성되었으며, 이 중에서도 Windows Prentation Foundation(WPF) 기반으로 작성되었다. 따라서 WPF framework의 구조와 이에 따른 프로그래밍 패턴에 대한 이해가 필요하다. 또한 project에서 가장 중심이 되는 visualization 성능을 향상시키기 위해서 사용되는 algorithm들을 조사하고 사용될만한 것들을 추려보았다.

## Windows Presentation Foundtation

![MVVM Pattern](mvvm.png){#fig:mvvm}

WPF는 .NET framework에서 사용되는 최신의 GUI framework이다. 기존의 WinForms framework와 달리 design(XAML)과 내부 logic(C#)을 구분하여 독립적으로 작성하기 용이하고, 유지관리를 편리하게 할 수 있다. WPF를 사용하는 데에는 MVVM(Model-View-ViewModel) 패턴을 이해하는 것이 필수적이다. MVVM이란 내부 데이터와 로직을 결정하는 Model과, 유저 인터페이스를 디자인하고 이것의 상호관계를 서술하는 View, 그리고 이 사이에서 데이터와 인터페이스를 매핑하는 방법을 서술하는 ViewModel단으로 분리하여 작성하는 패턴이다.

Figure {@fig:mvvm}에서 보듯, ViewModel은 View에 대해 독립적으로 작성되며, 인터페이스와 데이터 간 binding을 통해서 표현방식을 지정해줄 수 있다. 그리고 또한 내부 Model들과 관계를 맺으면서 database에 대한 접근을 시도할 수 있다. 실제 프로그래밍에 있어서 중요한 지점은 View에서는 ViewModel에 쉽게 접근이 가능하지만, ViewModel에서 View의 specific한 요소들을 접근하는 방향은 허용되지 않는다는 것이다.

## Data Reduction Algorithms

Time Series Data를 plotting하는 데 있어서 수많은 점을 찍게되면 낭비가 발생한다. 이에 따라 정확성을 잃지 않으면서도 필수적인 점들을 적절하게 추출해내는 알고리즘이 project의 핵심적인 부분이라 할 수 있다. 이것은 data visualization분야에서 연구가 활발하게 되었던 주제이기 때문에 여러 논문들을 참조하여 주제와 연관된 내용들을 정리했다.

### Strip Algorithm

### Visvalingam-Whyatt Algorithm

### Min-max Decimation

## Database Model



# Goal & Requirements

이번 프로젝트의 목표는 네 가지로 정리할 수 있다. 첫째, DB를 구축해서 최적화된 수행 시간을 얻을 것이다. 빅데이터를 다루다보니 DB의 사용은 필수적이며 사측 담당자와의 협의를 통해서 그것을 전제하고 들어가는 것으로 협의가 끝났다. DB 종류나 설계는 자유로우며 당연히 DB를 사용하는 이유는 더 나은 수행 시간을 위한 것이므로 여러 시도를 통해 최적화된 결과를 얻어내야만 할 것이다.     

둘째, 해당 환경에서 문제없이 작동해야 한다. Operating System은 Windows 7의 64bit이고, 우리가 만들 Application은 해당 환경 하에서 안정적인 실행을 보장해야만 할 것이다. 

셋째, 다중 도식 기능과 확대, 축소 기능이 필요하다. 해당 Application은 더 나은 사용자 경험을 위해서 수집된 데이터를 비교할 부분 부분마다 다른 색으로 겹쳐보거나 확대, 축소하는 기능이 필요할 것이다. 이것은 몇 번이라도 반복해야하는 업무의 특성상 필수적인 것이다. 

넷째, Algorithm의 적용은 정확해야만 한다. 그 정확성을 평가하는 데에 있어서 중요한 것은 변곡점의 소실 여부이다. 이들 변곡점이 사소한 것 하나라도 소실된다면 이미 그 Visualization은 실패한 것이다. 

위와 같은 기준을 전부 충족시킨다면 프로젝트의 결과물은 평가기준인 6초보다 빠른 수행시간과 정확성을 담보하면서도, 다중 도식 기능과 확대, 축소 기능을 통해서 더 높은 사용자 경험을 주게 될 것이다. 이렇게 이것들이 이번 프로젝트에서 가장 중요한 것이며 반드시 숙지하고 프로젝트를 진행해야만 할 것이다. 

## 프로젝트 개발 환경 및 서비스 환경

# Project Architecture

![Project architecture](arch.png){#fig:arch}

Figure {@fig:arch}에서 보이듯 project 구성은 크게 두 가지 application으로 나뉘어있다. data를 읽어들여 적절한 형태로 database에 저장하는 Server Application과, 이 database를 기초로 user에게 visualization을 제공하는 Desktop application이 그것이다. 양 측 모두 UI interaction이 필요할 뿐 dataflow 자체는 한방향으로 이루어진 단순한 구조를 띠고 있다. 이 둘을 나누어 각각 설명하도록 한다.

## Data Reducer

Server단에서는 Sensor Data를 파싱하고, data reduction을 수행하며, 이를 database에 넣는 것까지가 그 역할에 해당한다.

### CSV Parsing Module

CSV파일을 읽어 미리 정의된 형식의 data class로 저장시켜주는 module이다. 읽어들인 data는 Data Reduction Module의 input이 된다.

### Data Reduction Module

읽어들인 data에 대해서 각 센서별 데이터에 대해서 차후에 빠르게 visualize할 수 있도록 data reduction을 수행하는 module이다. CSV Parsing Module로부터 data를 받아서 이를 각각의 data reduction module에 넣어주고, 이를 통해서 특정 배율에서 각 point의 포함여부를 받아와 정리하게 된다. 이렇게 분석된 data는 Database Interface Module의 input이 된다.

### Database Interface Module

Database와의 통신을 담당하는 module로, data를 읽어들여 저장할 table의 생성부터, 분석된 data를 insert하는 역할도 맡는다. 이것을 끝으로 Server Application의 역할이 끝나게 된다.

## Data Visualizer

### Database Interface Module

Database에서 특정 wafer(table에 해당함)와 sensor의, 특정한 영역에 해당하는 data를 읽어올 수 있도록 interface를 제공하는 module이다. 이를 통해서 application에서 database에 접근하게 된다.

### Cache Controller Module

Display에 표시될 내용이 담긴 cache를 관리한다. 필요할 때에는 Database Interface Module에 요청을 보내 cache를 업데이트하게 된다. 이렇게 업데이트된 data는 Data Interpolator Module을 거쳐 다듬어지고, 이후에 User Display에 업데이트된다.

### Data Interpolator Module

data reduction을 통해서 sparse해진 Series Data의 사이를 추정하여 이어주는 역할을 하는 module이다. Cache Controller의 요청에 응답하는 방식이다.

### User Display

UI View에 해당하는 내용으로, 여기서는 오픈소스 라이브러리인 **OxyPlot**을 사용하여 visualization 및 interface를 제공하였다.

# Implementation Spec

## Data Reducer

![Data Reducer - UI](ui_reducer.png){#fig:uir}

![Data Reducer - Class Diagram](class_reducer.png){#fig:clr}

Figure {@fig:uir}은 Data Reducer의 UI를 보여주고 있다.

## Data Visualizer

![Data Visualizer - UI](ui_vis.png){#fig:uiv}

![Data Visualizer - Class Diagram](class_vis.png){#fig:clv}

Figure {@fig:uiv}은 Data Visualizer의 UI를 보여주고 있다.

## Used Libraries

- **CSVHelper** : CSV파일 parsing에 사용
- **OxyPlot** : WPF에 맞는 Graph Control제공

# Current Status

# Future Work

# Division & Assignment of Work

![Division & Assignment of Work](ass.png){#fig:ass}

Figure {@fig:ass}를 참조한다.

# Schedule

![Schedule](schedule.png){#fig:sch}

Figure {@fig:sch}를 참조한다.

# Demo Plan

\appendix
\section{(Appendix) Detailed Implementation Spec} \label{App:AppendixA}

## abc