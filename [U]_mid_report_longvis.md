% Long Time Process Data Visualization
% U조 김민수 조용훤
% 2016. 10. 28
---
toc: 1
---

\maketitle

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

Figure {@fig:mvvm}에서 보듯, ViewModel은 View에 대해 독립적으로 작성되며, 인터페이스와 데이터 간 바인딩을 통해서 표현방식을 지정해줄 수 있다.

## Data Reduction Algorithms

## Database Model

# Goal & Requirements

## 프로젝트 개발 환경 및 서비스 환경

# Project Architecture

![Project architecture](arch.png){#fig:arch}

# Implementation Sepc

# Current Status

# Future Work

# Division & Assignment of Work

# Schedule

# Demo Plan

# [Appendix] Detailed Implementation Spec